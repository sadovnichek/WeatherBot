using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotInfrastructure
{
    // Fluent interface for adding commands?

    public class TelegramBot
    {
        private readonly TelegramBotClient _bot;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<TelegramBot> _logger;

        public TelegramBot(ICommandHandler commandHandler,
            string token,
            ILogger<TelegramBot> logger)
        {
            _bot = new TelegramBotClient(token);
            _commandHandler = commandHandler;
            _logger = logger;
        }

        public async Task ReceiveAsync(Update update)
        {
            try
            {
                if (update != null && update.Message != null 
                    && update.Message.Text != null
                    && update.Message.Chat != null)
                {
                    var messageTextTokens = update.Message?.Text?.Split(" ");
                    var command = messageTextTokens?[0].Trim();
                    var args = messageTextTokens?.Skip(1).ToArray();
                    var chatId = update.Message.Chat.Id;

                    if (!_commandHandler.TryGetCommand(command, out var instance))
                    {
                        await SendMessage(chatId, "Неизвестная команда");
                        return;
                    }

                    var reply = await instance.Execute(args);
                 
                    while (reply != null)
                    {
                        var text = reply.BuildMessage();
                        await SendMessage(chatId, text);
                        reply = reply.Next;
                    }

                    _logger.LogDebug("Username:{username}\nName:{name}\nRequest:{request}\nTime UTC:{time}\n",
                        update.Message.Chat.Username,
                        update.Message.Chat.FirstName,
                        update.Message.Text,
                        DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception:{message}\nStackTrace:{stacktrace}\n",
                    ex.Message,
                    ex.StackTrace);

                await SendMessage(update.Message.Chat.Id, PlainReply.OnError().BuildMessage());
            }
        }

        private async Task SendMessage(long chatId, string message)
        {
            await _bot.SendMessage(chatId,
                    message,
                    Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}