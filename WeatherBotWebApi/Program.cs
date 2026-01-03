using Telegram.Bot.Types;
using WeatherBotDomain;

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

var bot = new WeatherBot(client, new WeatherCore(), token);

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();