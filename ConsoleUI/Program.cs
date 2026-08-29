using BotInfrastructure;
using System.Net;
using BotInfrastructure.Database;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using WeatherBotDomain;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Services;

namespace ConsoleUI
{
    public class Program
    {
        public static async Task Main()
        {
            var proxy = new WebProxy("https://160.153.172.106:3193")
            {
                BypassProxyOnLocal = false
            };
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            using var db = new AppDbContext();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //var proxy = new WebProxy("https://104.248.63.15:30588");

            var handler = new HttpClientHandler()
            {
                UseProxy = false,
                Proxy = proxy,
            };

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast")
            };

            var domain = new WeatherCore();

            var client = new WeatherApiClient(httpClient);
            var controller = new OpenMeteoController();
            var cityResolverService = new CityResolverService(db);

            var coordinateArgumentResolver = new CoordinateArgumentResolver(cityResolverService);

            var commands = new Dictionary<string, ICommand>();
            commands.Add("/start", new StartCommand());
            commands.Add("/today", new TodayCommand(controller, client, domain, coordinateArgumentResolver));
            commands.Add("/tomorrow", new TomorrowCommand(controller, client, domain, coordinateArgumentResolver));
            commands.Add("/hourly", new HourlyCommand(controller, client, domain, coordinateArgumentResolver));
            commands.Add("/daytime", new DaytimeCommand(controller, client, coordinateArgumentResolver));
            commands.Add("/weekly", new WeeklyCommand(client, controller, domain, coordinateArgumentResolver));
            commands.Add("/help", new HelpCommand(commands));

            var city = "Москва";
            
            if (!cityResolverService.TryResolve(city, out var coordinates))
            {
                Console.WriteLine($"City {city} not found");
                Environment.Exit(0);
            }

            var latitude = coordinates.Latitude.ToString();
            var longitude = coordinates.Longitude.ToString();

            Console.WriteLine(latitude);
            Console.WriteLine(longitude);

            var reply = await commands["/today"].Execute([city]);

            while (reply != null)
            {
                var text = reply.BuildMessage();
                Console.WriteLine(text);
                reply = reply.Next;
            }
        }
    }
}