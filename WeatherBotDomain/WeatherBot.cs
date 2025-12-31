using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherBotDomain
{
    public class CustomerUpdate
    {
        public Update Update { get; set; }
    }

    public class WeatherBot
    {
        private string token;
        private TelegramBotClient client;

        public WeatherBot()
        {
            var token = GetToken("../../../token.txt");
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

        private static string GetToken(string filepath)
        {
            if (!File.Exists(filepath))
                throw new ArgumentException($"Unable to find the file with token {filepath}");
            using var reader = new StreamReader(filepath);
            return reader.ReadLine();
        }
    }
}
