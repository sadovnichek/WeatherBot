using BotInfrastructure;
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

            var domain = new WeatherCore();

            var bus = Channel.CreateUnbounded<string>();

            var commands = new Dictionary<string, ICommand>();
            commands.Add("/start", new StartCommand(bus));
            commands.Add("/today", new TodayCommand(client, domain, bus, uri));
            commands.Add("/tomorrow", new TomorrowCommand(client, domain, bus, uri));
            commands.Add("/hourly", new HourlyCommand(client, uri, bus, domain));
            commands.Add("/daytime", new DaytimeCommand(client, uri, bus));
            commands.Add("/help", new HelpCommand(commands, bus));

            var commandHandler = new CommandHandler(commands);

            await commandHandler.HandleCommand("/today", ["10", "15"]);

            while (bus.Reader.Count > 0)
            {
                var reply = await bus.Reader.ReadAsync();
                Console.WriteLine(reply);
            }
        }
    }
}