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

var commands = new Dictionary<string, ICommand>()
{
    {  "/time", new TimeCommand() },
    {  "/today", new TodayCommand(client, domain, uri) },
    {  "/tomorrow", new TomorrowCommand(client, domain, uri) }
};

var commandHandler = new CommandHandler(commands);

var bot = new Bot(commandHandler, token);

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.MapPost("/scheduled-work", async (UserRequest r) =>
    {
        await Task.Yield();
    }
);

app.Run();

public record UserRequest(long chatId);