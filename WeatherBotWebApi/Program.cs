using BotInfrastructure;
using System.Collections.Frozen;
using System.Threading.Channels;
using Telegram.Bot.Types;
using WeatherBotDomain;
using WeatherBotDomain.Commands;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");

var handler = new HttpClientHandler()
{
    UseProxy = false,
};

var client = new HttpClient(handler)
{
    Timeout = new TimeSpan(0, 0, 5)
};

var domain = new WeatherCore();

var uri = "https://api.open-meteo.com/v1/forecast";

var bus = Channel.CreateUnbounded<string>();

var commands = new Dictionary<string, ICommand>()
{
    {  "/start", new StartCommand(bus) },
    {  "/today", new TodayCommand(client, domain, bus, uri) },
    {  "/tomorrow", new TomorrowCommand(client, domain, bus, uri) },
    {  "/hourly", new HourlyCommand(client, uri, bus, domain) },
    {  "/daytime", new DaytimeCommand(client, uri, bus) }
}.ToFrozenDictionary();

var help = new HelpCommand(commands, bus);

var commandHandler = new CommandHandler(commands, help);

var bot = new TelegramBot(commandHandler, bus, token);

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();