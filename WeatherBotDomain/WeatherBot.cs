using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class WeatherBot
    {
        private TelegramBotClient client;

        public WeatherBot(string token)
        {
            client = new TelegramBotClient(token);
        }

        public async Task ReceiveAsync(Update update)
        {
            if(update.Message != null)
            {
                var message = update.Message;
                await client.SendMessage(message.Chat.Id, message.Text);
            }
        }
    }
}
