using BotInfrastructure;
using Newtonsoft.Json;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(HttpClient client, WeatherCore domain, IMessageBus<Message> bus, string uri) 
            : base(client, domain, bus, uri)
        {

        }

        public override string Description => "Погода и температура сегодня";

        protected override WeatherReply ProcessResponse(string jsonResponse)
        {
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(jsonResponse);

            var utcOffset = parsedJson.UtcOffsetSeconds;
            var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

            var temperatures = parsedJson.Data.TemperaturePoints.Take(24).ToArray();
            var weatherCodes = parsedJson.Data.WeatherCodes.Take(24).ToArray();

            return weatherDomain.GetReply("Сегодня", timeNow, weatherCodes, temperatures);
        }
    }
}
