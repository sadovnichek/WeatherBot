using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class WeatherBot
    {
        private TelegramBotClient client;
        private static readonly Dictionary<string, ICommand> commands;

        static WeatherBot()
        {
            commands = new()
            {
                {  "/time", new TimeCommand() }
            };
        }

        public WeatherBot(string token)
        {
            client = new TelegramBotClient(token);
        }

        public async Task ReceiveAsync(Update update)
        {
            if(update.Message != null)
            {
                var messageText = update.Message.Text;

                if(commands.ContainsKey(messageText))
                {
                    var reply = HandleCommand(messageText, []);
                    await client.SendMessage(update.Message.Chat.Id, reply);
                }
            }
        }

        private string HandleCommand(string command, string[] args)
        {
            var handler = commands[command];
            return handler.Execute(args);
        }
    }
}
