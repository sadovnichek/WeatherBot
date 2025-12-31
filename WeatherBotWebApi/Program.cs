using WeatherBotDomain;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var bot = new WeatherBot();

app.MapPost("/webhook", async (CustomerUpdate u) =>
    {
        await bot.ReceiveAsync(u);
    }
);

app.Run();