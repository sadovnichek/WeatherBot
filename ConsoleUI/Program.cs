using BotInfrastructure;
using System.Collections.Frozen;
using System.Threading.Channels;
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

            var bus = Channel.CreateUnbounded<string>();

            var commands = new Dictionary<string, ICommand>()
            {
                {  "/time", new TimeCommand() },
                {  "/today", new TodayCommand(client, core, bus, uri) },
                {  "/tomorrow", new TomorrowCommand(client, core, bus, uri) },
                {  "/hourly", new HourlyCommand(client, uri, bus, core) },
                {  "/start", new StartCommand(bus) },
                {  "/daytime", new DaytimeCommand(client, uri, bus) }
            }.ToFrozenDictionary();

            var help = new HelpCommand(commands, bus);

            var commandHandler = new CommandHandler(commands, help);

            await commandHandler.HandleCommand("/hourly", []);

            while (bus.Reader.Count > 0)
            {
                var reply = await bus.Reader.ReadAsync();
                Console.WriteLine(reply);
            }
        }
    }
}