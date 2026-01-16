using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotInfrastructure
{
    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        private readonly CommandHandler commandHandler;

        public TelegramBot(CommandHandler handler,
            string token)
        {
            bot = new TelegramBotClient(token);
            commandHandler = handler;
        }

        //Sending several messages?
        public async Task ReceiveAsync(Update update)
        {
            if(update.Message != null && update.Message.Text != null)
            {
                var messageTextTokens = update.Message?.Text?.Split(" ");
                var command = messageTextTokens?[0].Trim();
                var args = messageTextTokens?.Skip(1).ToArray();

                if(commandHandler.IsCommandExists(command))
                {
                    var reply = await GetReply(command, args);
                    await bot.SendMessage(update.Message.Chat.Id, reply);
                }
            }
        }

        private async Task<string> GetReply(string command, string[] args)
        {
            if (command == "/help")
                return GetHelp();

            return await commandHandler.HandleCommand(command, args);
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