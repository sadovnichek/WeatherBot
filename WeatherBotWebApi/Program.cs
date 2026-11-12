using BotInfrastructure;
using BotInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Services;

var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration.GetValue("App:Port", 8080);
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddSingleton<IWeatherService, WeatherService>()
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
                .AddSingleton<IInfrastructureCommand, HelpCommand>()
                .AddSingleton<ICommandHandler, CommandHandler>()
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
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlite(connectionString);
                })
                .AddSingleton(s =>
                {
                    var handler = s.GetService<ICommandHandler>();
                    var token = builder.Configuration["Bot:Token"];
                    return new TelegramBot(handler, token);
                });

var app = builder.Build();

var bot = app.Services.GetService<TelegramBot>();

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();