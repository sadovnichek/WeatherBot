using BotInfrastructure;
using BotInfrastructure.Database;
using Microsoft.AspNetCore.HttpOverrides;
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
                .AddSingleton<ICommand, SetDefaultCityCommand>()
                .AddSingleton<IInfrastructureCommand, HelpCommand>()
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ICoordinateResolver, CoordinateResolver>()
                .AddSingleton(new BotToken(builder.Configuration["Bot:Token"]))
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
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlite(connectionString);
                })
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto;
                })
                .AddSingleton<TelegramBot>();

var app = builder.Build();

//auto migration
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseForwardedHeaders();

var bot = app.Services.GetRequiredService<TelegramBot>();

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();