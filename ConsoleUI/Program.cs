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

            var chatId = 0;
            var user = db.Users.FirstOrDefault(u => u.ChatId == chatId);

            if (user == null)
            {
                var entity = db.Users.Add(new User { ChatId = chatId });
                db.SaveChanges();
            }

            var reply = await commands["/today"].Execute(["Екатеринбург"]);

            while (reply != null)
            {
                var text = reply.BuildMessage();
                Console.WriteLine(text);
                reply = reply.Next;
            }
        }
    }
}