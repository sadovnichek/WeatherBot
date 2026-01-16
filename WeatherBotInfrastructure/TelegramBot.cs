using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotInfrastructure
{
    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        private readonly CommandHandler commandHandler;
        private readonly IMessageBus<string> messageBus;

        public TelegramBot(CommandHandler handler,
            IMessageBus<string> bus,
            string token)
        {
            bot = new TelegramBotClient(token);
            commandHandler = handler;
            messageBus = bus;
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
                    if(command == "/help")
                    {
                        var reply = GetHelp();
                        await bot.SendMessage(update.Message.Chat.Id, reply);
                    }

                    while(!messageBus.IsEmpty())
                    {
                        var reply = await messageBus.Obtain();
                        await bot.SendMessage(update.Message.Chat.Id, reply);
                    }
                }
            }
        }

        //private async Task<IEnumerable<string>> GetReply(long chatId, string command, string[] args)
        //{
        //    if (command == "/help")
        //        yield return GetHelp();

        //    await commandHandler.HandleCommand(chatId, command, args);
        //}

        private string GetHelp()
        {
            return string.Join("\n", 
                commandHandler
                .GetCommands()
                .Select(c => $"{c} {commandHandler.GetCommandDescription(c)}"));
        }
    }
}