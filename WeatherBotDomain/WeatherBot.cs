using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class CustomerUpdate
    {
        public Update Update { get; }
    }

    public class WeatherBot
    {
        private TelegramBotClient client;

        public WeatherBot(string token)
        {
            client = new TelegramBotClient(token);
        }

        public async Task ReceiveAsync(CustomerUpdate update)
        {
            if(update.Update.Message != null)
            {
                var message = update.Update.Message;
                await client.SendMessage(message.Chat.Id, message.Text);
            }
        }
    }
}
