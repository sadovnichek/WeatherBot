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

            var bus = new MessageBus<Message>();

            var command = new TodayCommand(client, core, bus, uri);

            await command.Execute([]);

            var reply = await bus.Obtain();

            Console.WriteLine(reply);
        }
    }
}