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
            if (update.Message != null && update.Message.Text != null)
            {
                var messageTextTokens = update.Message?.Text?.Split(" ");
                var command = messageTextTokens?[0].Trim();
                var args = messageTextTokens?.Skip(1).ToArray();

                if (commandHandler.IsCommandExists(command))
                {
                    await commandHandler.HandleCommand(command, args);

                    while (!messageBus.IsEmpty())
                    {
                        var reply = await messageBus.Obtain();
                        await bot.SendMessage(update.Message.Chat.Id, reply);
                    }
                }
            }
        }
    }
}