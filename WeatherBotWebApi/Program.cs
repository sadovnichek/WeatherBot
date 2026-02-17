using BotInfrastructure;
using Telegram.Bot.Types;
using WeatherBotDomain;
using WeatherBotDomain.Commands;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");

using var handler = new HttpClientHandler()
{
    UseProxy = false,
};

using var client = new HttpClient(handler)
{
    Timeout = new TimeSpan(0, 0, 5)
};

var domain = new WeatherCore();

var uri = "https://api.open-meteo.com/v1/forecast";

var commands = new Dictionary<string, ICommand>();
commands.Add("/start", new StartCommand());
commands.Add("/today", new TodayCommand(client, domain, uri));
commands.Add("/tomorrow", new TomorrowCommand(client, domain, uri));
commands.Add("/hourly", new HourlyCommand(client, uri, domain));
commands.Add("/daytime", new DaytimeCommand(client, uri));
commands.Add("/help", new HelpCommand(commands));

var bot = new TelegramBot(new CommandHandler(commands), token);

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();