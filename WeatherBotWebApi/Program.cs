using WeatherBotDomain;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var token = Environment.GetEnvironmentVariable("BOT_TOKEN");

var bot = new WeatherBot(token);

app.MapPost("/webhook", async (CustomerUpdate u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();