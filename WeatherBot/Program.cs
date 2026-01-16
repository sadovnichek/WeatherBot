using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BotInfrastructure;
using BotInfrastructure.Commands;

namespace WeatherBot
{
    public class Program
    {
        private static TelegramBot bot;

        static async Task Main()
        {
            var token = Environment.GetEnvironmentVariable("TEST_BOT_TOKEN");

            var handler = new HttpClientHandler()
            {
                UseProxy = false,
            };

            var client = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 0, 5)
            };

            var core = new WeatherCore();

            var uri = "https://api.open-meteo.com/v1/forecast";

            var commandHandler = new CommandHandler();
            commandHandler.RegisterCommand("/today", new TodayCommand(client, core, uri));

            bot = new TelegramBot(commandHandler, token);

            bot.OnError += Bot_OnError;
            bot.OnMessage += Bot_OnMessage;
            bot.OnUpdate += Bot_OnUpdate;

            Console.ReadKey();
        }

        private static async Task Bot_OnUpdate(Update update)
        {
            Console.WriteLine("Got update");
        }

        private static async Task Bot_OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
        }

        private static async Task Bot_OnMessage(Message message, UpdateType type)
        {
            
        }
    }
}