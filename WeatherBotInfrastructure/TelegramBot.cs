using System.Collections.Frozen;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotInfrastructure
{
    // Fluent interface for adding commands?

    public class TelegramBot
    {
        private readonly TelegramBotClient bot;
        private FrozenDictionary<string, ICommand> botCommands;

        public TelegramBot(Dictionary<string, ICommand> commands,
            string token)
        {
            bot = new TelegramBotClient(token);
            botCommands = commands.ToFrozenDictionary();
        }

        public async Task ReceiveAsync(Update update)
        {
            try
            {
                if (update.Message != null && update.Message.Text != null)
                {
                    var messageTextTokens = update.Message?.Text?.Split(" ");
                    var command = messageTextTokens?[0].Trim();
                    var args = messageTextTokens?.Skip(1).ToArray();

                    if (!botCommands.TryGetValue(command, out var instance))
                    {
                        await bot.SendMessage(update.Message.Chat.Id,
                                "Неизвестная команда",
                                Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        return;
                    }

                    var reply = await instance.Execute(args);
                 
                    while (reply != null)
                    {
                        var text = reply.BuildMessage();
                        await bot.SendMessage(update.Message.Chat.Id,
                            text,
                            Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        reply = reply.Next;
                    }

                    Console.WriteLine($"Username: {update.Message.Chat.Username}\n" +
                        $"{update.Message.Chat.FirstName} {update.Message.Chat.LastName}\n" +
                        $"Request: {update.Message.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                await bot.SendMessage(update.Message.Chat.Id,
                    PlainReply.OnError().BuildMessage(),
                    Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }
}