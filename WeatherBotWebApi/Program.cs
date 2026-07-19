using BotInfrastructure;
using Telegram.Bot.Types;
using WeatherBotDomain;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");

var handler = new HttpClientHandler()
{
    UseProxy = false,
};

using var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast")
};

var domain = new WeatherCore();

var apiClient = new WeatherApiClient(httpClient);
var controller = new OpenMeteoController();

var commands = new Dictionary<string, ICommand>();
commands.Add("/start", new StartCommand());
commands.Add("/today", new TodayCommand(controller, apiClient, domain));
commands.Add("/tomorrow", new TomorrowCommand(controller, apiClient, domain));
commands.Add("/hourly", new HourlyCommand(controller, apiClient, domain));
commands.Add("/daytime", new DaytimeCommand(controller, apiClient));
commands.Add("/help", new HelpCommand(commands));

var bot = new TelegramBot(commands, token);

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();