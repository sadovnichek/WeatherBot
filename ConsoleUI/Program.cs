using BotInfrastructure;
using Telegram.Bot.Types;
using WeatherBotDomain;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;

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

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast")
            };

            var domain = new WeatherCore();

            var controller = new OpenMeteoController(client);

            var commands = new Dictionary<string, ICommand>();
            commands.Add("/start", new StartCommand());
            commands.Add("/today", new TodayCommand(controller, domain));
            commands.Add("/tomorrow", new TomorrowCommand(controller, domain));
            commands.Add("/hourly", new HourlyCommand(controller, domain));
            commands.Add("/daytime", new DaytimeCommand(controller));
            commands.Add("/help", new HelpCommand(commands));

            var reply = await commands["/today"].Execute([]);

            while (reply != null)
            {
                var text = reply.BuildMessage();
                Console.WriteLine(text);
                reply = reply.Next;
            }
        }
    }
}