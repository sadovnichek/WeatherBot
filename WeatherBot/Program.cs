using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeatherBot
{
    public enum State
    {
        None,
        Location,
        Time,
        Ready
    }

    public class Program
    {
        private static TelegramBotClient bot;
        private static DataBaseStub database;
        private static Regex extractTimeRegexp;
        private static string help;
        private static Random random;
        private static State currentState;

        static Program()
        {
            database = new DataBaseStub();
            extractTimeRegexp = new Regex(@"\d{1,2}:\d\d");
            help = "*Weather Bot*\n/time set up time\n/help shows this help";
            random = new Random();
            currentState = State.None;
        }

        static async Task Main()
        {
            var token = GetToken("../../../secrets.txt");

            using var cts = new CancellationTokenSource();
            bot = new TelegramBotClient(token, cancellationToken: cts.Token);
            var me = await bot.GetMe();
            Console.WriteLine($"Started {me.FirstName}");

            bot.OnError += Bot_OnError;
            bot.OnMessage += Bot_OnMessage;
            bot.OnUpdate += Bot_OnUpdate;

            Console.ReadKey();
            database.CloseConnection();
            cts.Cancel();
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
            else if (message.Location is not null)
            {
                currentState = State.Location;
                await HandleLocation(message);
            }
            else if (message.Text == "/time")
            {
                currentState = State.Time;
                await Reply(message, "Specify time as hh:mm...");
            }
            else if (currentState == State.Time)
            {
                await ValidateTime(message);
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
            database.Update(message.From.Id, new DataBaseRecord { TelegramId = message.From.Id });
            await bot.SendMessage(message.Chat, "Hi! I'm the Weater Bot!");
            await bot.SendMessage(message.Chat, "First, I need to know your location...",
                replyMarkup: new KeyboardButton[] { KeyboardButton.WithRequestLocation("Share Location") });
        }

        private static async Task HandleLocation(Message message)
        {
            var record = database.Read(message.From.Id);
            record.Latitude = message.Location.Latitude;
            record.Longitude = message.Location.Longitude;
            database.Update(message.From.Id, record);

            await Reply(message, $"Great! Now I know your location");
            await Reply(message, "Now set time by command /time...");
        }

        private static async Task ValidateTime(Message message)
        {
            var regexpMatch = extractTimeRegexp.Match(message.Text);
            if (!regexpMatch.Success)
                await Reply(message, "Wrong time format. Try type as hh:mm...");
            else
            {
                var time = regexpMatch.Value;

                var record = database.Read(message.From.Id);
                record.TimeToWakeup = TimeOnly.Parse(time);
                database.Update(message.From.Id, record);

                await Reply(message, "Time was set succefully!");
                await Reply(message, $"You will get a message at {time}");
                currentState = State.Ready;
            }
        }

        private static async Task SendHelp(Message message)
        {
            await bot.SendMessage(message.Chat, help, ParseMode.Markdown);
        }

        private static async Task Reply(Message message, string text)
        {
            var millisecondsDelay = (int)GetNormalDistributionValue(100 * text.Length, 1/12f);
            await Task.Delay(millisecondsDelay);
            await bot.SendMessage(message.Chat, text);
        }

        private static double GetNormalDistributionValue(double mean, double std)
        {
            return std * Math.Cos(2 * Math.PI * random.NextDouble()) * Math.Sqrt(-2 * Math.Log(random.NextDouble())) + mean;
        }

        private static string GetToken(string filepath)
        {
            if (!File.Exists(filepath))
                throw new ArgumentException($"Unable to find the file with token {filepath}");
            using var reader = new StreamReader(filepath);
            return reader.ReadLine();
        }
    }
}