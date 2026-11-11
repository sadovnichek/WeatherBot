using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotInfrastructure
{
    // Fluent interface for adding commands?

    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        //private FrozenDictionary<string, ICommand> botCommands;
        private readonly ICommandHandler _commandHandler;

        public TelegramBot(ICommandHandler commandHandler,
            string token)
        {
            bot = new TelegramBotClient(token);
            //botCommands = commands.ToFrozenDictionary(c => c.Name, c => c);
            _commandHandler = commandHandler;
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

                    Console.WriteLine($"Username: {update.Message.Chat.Username} " +
                        $"First name: {update.Message.Chat.FirstName} Last name: {update.Message.Chat.LastName} " +
                        $"Request: {update.Message.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                await SendMessage(update.Message.Chat.Id, PlainReply.OnError().BuildMessage());
            }
        }

        private async Task SendMessage(long chatId, string message)
        {
            await bot.SendMessage(chatId,
                    message,
                    Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}