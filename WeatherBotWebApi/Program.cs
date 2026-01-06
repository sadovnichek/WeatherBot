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
    {  "/weather", new WeatherCommand(client, domain, uri) }
};

var commandHandler = new CommandHandler(commands);

var bot = new WeatherBot(commandHandler, token);

app.MapPost("/webhook", async (Update u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

//app.MapPost("/cron/test", async (HttpContext ctx) =>
//{
//    if (ctx.Request.Headers["X-Cron-Key"] != Environment.GetEnvironmentVariable("CRON_KEY"))
//        return Results.Unauthorized();

//    await bot.ReceiveAsync(new Update() { Message = new Message() { Text = "/weather", Chat = new Chat() { Id = 0 } } });

//    return Results.Ok();
//});

app.Run();