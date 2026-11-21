using BotInfrastructure;
using BotInfrastructure.Database;
using System.Globalization;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Services;
using Microsoft.Extensions.DependencyInjection;
using WeatherBotDomain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsoleUI
{
    public class Program
    {
        public static async Task Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            var services = new ServiceCollection()
                .AddSingleton<IWeatherService, WeatherService>()
                .AddSingleton<IPrecipitationService, PrecipitationService>()
                .AddSingleton<IWeatherApiClient, WeatherApiClient>()
                .AddSingleton<IWeatherApiController, OpenMeteoController>()
                .AddSingleton<ICityResolverService, CityResolverService>()
                .AddSingleton<ICommand, StartCommand>()
                .AddSingleton<ICommand, TodayCommand>()
                .AddSingleton<ICommand, TomorrowCommand>()
                .AddSingleton<ICommand, HourlyCommand>()
                .AddSingleton<ICommand, DaytimeCommand>()
                .AddSingleton<ICommand, WeeklyCommand>()
                .AddSingleton<ICommand, SetDefaultCityCommand>()
                .AddSingleton<IInfrastructureCommand, HelpCommand>()
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ICoordinateResolver, CoordinateResolver>()
                .AddLogging(builder => builder.AddConsole())
                .AddSingleton(s =>
                {
                    var handler = new HttpClientHandler();
                    return new HttpClient(handler)
                    {
                        BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast")
                    };
                })
                .AddDbContext<AppDbContext>(options =>
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeatherBotStorage", "wb.db");
                    options.UseSqlite($"Data Source = {path}");
                })
                .BuildServiceProvider();

            var handler = services.GetService<ICommandHandler>();
            handler.TryGetCommand("/weekly", out var command);
            var userInfo = new UserInfo() { ChatId = 0, LastMessageTimeUtc = DateTime.UtcNow.ToUnixEpoch() };

            var reply = await command.Execute(userInfo, [/*"Екатеринбург"*/]);

            while (reply != null)
            {
                var text = reply.BuildMessage();
                Console.WriteLine(text);
                reply = reply.Next;
            }

            var service = services.GetRequiredService<IUserService>();
            service.GetAllUsers()
                .ToList()
                .ForEach(u => Console.Write(u));
        }
    }
}