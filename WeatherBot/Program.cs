using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeatherBot
{
    public class Program
    {
        private static TelegramBotClient bot;
        private static string help;

        static Program()
        {
            help = "*Weather Bot*\n/time set up time\n/help shows this help";
        }

        static async Task Main()
        {
            var token = Environment.GetEnvironmentVariable("BOT_TOKEN");

            bot = new TelegramBotClient(token);
            var me = await bot.GetMe();
            Console.WriteLine($"Started {me.FirstName}");

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
            if (message.Text == "/start")
            {
                await HandleStartCommand(message);
            }
            else if (message.Text == "/time")
            {
                await Reply(message, "Specify time as hh:mm...");
            }
            else if (message.Text == "/help")
            {
                await SendHelp(message);
            }
            else
            {
                await SendHelp(message);
            }
        }

        private static async Task HandleStartCommand(Message message)
        {
            await bot.SendMessage(message.Chat, "Hi! I'm the Weather Bot!");
            await bot.SendMessage(message.Chat, "First, I need to know your location...",
                replyMarkup: new KeyboardButton[] { KeyboardButton.WithRequestLocation("Share Location") });
        }

        private static async Task SendHelp(Message message)
        {
            await bot.SendMessage(message.Chat, help, ParseMode.Markdown);
        }

        private static async Task Reply(Message message, string text)
        {
            await bot.SendMessage(message.Chat, text);
        }
    }
}