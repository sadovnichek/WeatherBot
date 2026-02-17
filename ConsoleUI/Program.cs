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

            var commands = new Dictionary<string, ICommand>();
            commands.Add("/start", new StartCommand());
            commands.Add("/today", new TodayCommand(client, domain, uri));
            commands.Add("/tomorrow", new TomorrowCommand(client, domain, uri));
            commands.Add("/hourly", new HourlyCommand(client, uri, domain));
            commands.Add("/daytime", new DaytimeCommand(client, uri));
            commands.Add("/help", new HelpCommand(commands));

            var commandHandler = new CommandHandler(commands);

            await foreach (var reply in commandHandler.HandleCommand("/today", ["10", "15"]))
            {
                Console.WriteLine(reply.BuildMessage());
            }
        }
    }
}