using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class WeatherBot
    {
        private readonly TelegramBotClient bot;
        private readonly Dictionary<string, ICommand> commands;

        public WeatherBot(HttpClient client, 
            WeatherDomain domain,
            string token)
        {
            bot = new TelegramBotClient(token);
            commands = new()
            {
                {  "/time", new TimeCommand() },
                {  "/weather", new WeatherCommand(client, domain) }
            };
        }

        public async Task ReceiveAsync(Update update)
        {
            if(update.Message != null)
            {
                var messageText = update.Message.Text;

                if(commands.ContainsKey(messageText))
                {
                    var reply = await HandleCommand(messageText, []);
                    await bot.SendMessage(update.Message.Chat.Id, reply);
                }
            }
        }

        private async Task<string> HandleCommand(string command, string[] args)
        {
            var handler = commands[command];
            return await handler.Execute(args);
        }
    }
}