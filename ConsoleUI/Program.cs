using BotInfrastructure;
using WeatherBotDomain;
using WeatherBotDomain.Commands;

namespace ConsoleUI
{
    public class Program
    {
        public static async Task Main()
        {
            var handler = new HttpClientHandler()
            {
                UseProxy = false,
            };

            var client = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 0, 5)
            };

            var uri = "https://api.open-meteo.com/v1/forecast";

            var core = new WeatherCore();

            var bus = new MessageBus<string>();

            var commands = new Dictionary<string, ICommand>()
            {
                {  "/time", new TimeCommand() },
                {  "/today", new TodayCommand(client, core, bus, uri) },
                {  "/tomorrow", new TomorrowCommand(client, core, bus, uri) },
                {  "/hourly", new HourlyCommand(client, uri, bus, core) }
            };

            var commandHandler = new CommandHandler(commands, bus);

            await commandHandler.HandleCommand("/hourly", []);

            while (!bus.IsEmpty())
            {
                var reply = await bus.Get();
                Console.WriteLine(reply);
            }
        }
    }
}