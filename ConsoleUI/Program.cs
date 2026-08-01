using BotInfrastructure;
<<<<<<< HEAD
using System.Net;
=======
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
>>>>>>> 1baad8a (create simple db connection)
using System.Globalization;
using WeatherBotDomain;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;

namespace ConsoleUI
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        public long ChatId { get; set; }

        public long CityId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class City
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./../../../app.db");
        }
    }

    public class Program
    {
        public static async Task Main()
        {
<<<<<<< HEAD
            var proxy = new WebProxy("https://160.153.172.106:3193")
            {
                BypassProxyOnLocal = false
            };
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
=======
            using var db = new AppDbContext();
>>>>>>> 1baad8a (create simple db connection)

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

            var commands = new Dictionary<string, ICommand>();
            commands.Add("/start", new StartCommand());
            commands.Add("/today", new TodayCommand(controller, client, domain));
            commands.Add("/tomorrow", new TomorrowCommand(controller, client, domain));
            commands.Add("/hourly", new HourlyCommand(controller, client, domain));
            commands.Add("/daytime", new DaytimeCommand(controller, client));
            commands.Add("/weekly", new WeeklyCommand(client, controller, domain));

            commands.Add("/help", new HelpCommand(commands));

            var reply = await commands["/start"].Execute(["56.82", "60.55"]);

            while (reply != null)
            {
                var text = reply.BuildMessage();
                Console.WriteLine(text);
                reply = reply.Next;
            }

            //db.Users.Add(new User() { ChatId = 1, Latitude = 56.82, Longitude = 60.55 });
            db.SaveChanges();
        }
    }
}