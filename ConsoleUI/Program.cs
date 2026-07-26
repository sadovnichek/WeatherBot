using BotInfrastructure;
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

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast")
            };

            var domain = new WeatherCore();

            var client = new WeatherApiClient(httpClient);
            var controller = new OpenMeteoController();

            var commands = new Dictionary<string, ICommand>();
            commands.Add("/start", new StartCommand());
            commands.Add("/today", new TodayCommand(controller, client, domain));
            commands.Add("/tomorrow", new TomorrowCommand(controller, client, domain));
            commands.Add("/hourly", new HourlyCommand(controller, client, domain));
            commands.Add("/daytime", new DaytimeCommand(controller, client));
            commands.Add("/weekly", new WeeklyCommand(client, controller, domain));

            commands.Add("/help", new HelpCommand(commands));

            var reply = await commands["/weekly"].Execute([]);

            while (reply != null)
            {
                var text = reply.BuildMessage();
                Console.WriteLine(text);
                reply = reply.Next;
            }
        }
    }
}