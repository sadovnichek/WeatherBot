using System.Threading.Channels;
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

                await foreach (var reply in 
                    commandHandler.HandleCommand(command, args))
                {
                    var text = reply.BuildMessage();
                    await bot.SendMessage(update.Message.Chat.Id,
                        text,
                        Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
            }
        }
    }
}