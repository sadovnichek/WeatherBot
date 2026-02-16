using System.Threading.Channels;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotInfrastructure
{
    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        private readonly CommandHandler commandHandler;
        private readonly ChannelReader<string> messageBus;

        public TelegramBot(CommandHandler handler,
            ChannelReader<string> bus,
            string token)
        {
            bot = new TelegramBotClient(token);
            commandHandler = handler;
            messageBus = bus;
        }

        public async Task ReceiveAsync(Update update)
        {
            if (update.Message != null && update.Message.Text != null)
            {
                var messageTextTokens = update.Message?.Text?.Split(" ");
                var command = messageTextTokens?[0].Trim();
                var args = messageTextTokens?.Skip(1).ToArray();

                if (!commandHandler.IsCommandExists(command))
                {
                    await bot.SendMessage(update.Message.Chat.Id,
                            "Неизвестная команда",
                            Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    return;
                }

                await commandHandler.HandleCommand(command, args);

                while (messageBus.Count > 0)
                {
                    var reply = await messageBus.ReadAsync();
                    await bot.SendMessage(update.Message.Chat.Id,
                        reply,
                        Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
            }
        }
    }
}