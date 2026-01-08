using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class Bot
    {
        private readonly TelegramBotClient bot;
        private readonly CommandHandler commandHandler;

        public Bot(CommandHandler handler,
            string token)
        {
            bot = new TelegramBotClient(token);
            commandHandler = handler;
        }

        public async Task ReceiveAsync(Update update)
        {
            if(update.Message != null && update.Message.Text != null)
            {
                var messageTextTokens = update.Message?.Text?.Split(" ");
                var command = messageTextTokens?[0].Trim();
                var args = messageTextTokens?.Skip(1).ToArray();

                if(command == "/help")
                {
                    var reply = GetHelp();
                    await bot.SendMessage(update.Message.Chat.Id, reply);
                    Console.WriteLine(update.Message.Chat.Id);
                }

                else if (commandHandler.IsCommandExists(command))
                {
                    var reply = await commandHandler.HandleCommand(command, args);
                    await bot.SendMessage(update.Message.Chat.Id, reply);
                }
            }
        }

        private string GetHelp()
        {
            return string.Join("\n", 
                commandHandler
                .GetCommands()
                .Select(c => $"{c} {commandHandler.GetCommandDescription(c)}"));
        }
    }
}