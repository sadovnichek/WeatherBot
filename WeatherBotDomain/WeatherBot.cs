using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class WeatherBot
    {
        private readonly TelegramBotClient bot;
        private readonly CommandHandler commandHandler;
        private readonly ICommand helpCommand;

        public WeatherBot(CommandHandler handler,
            string token)
        {
            bot = new TelegramBotClient(token);
            commandHandler = handler;
            helpCommand = new HelpCommand(commandHandler);
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
                    var reply = await helpCommand.Execute([]);
                    await bot.SendMessage(update.Message.Chat.Id, reply);
                    return;
                }

                if(commandHandler.IsCommandExists(command))
                {
                    var reply = await commandHandler.HandleCommand(command, args);
                    await bot.SendMessage(update.Message.Chat.Id, reply);
                }
            }
        }
    }
}