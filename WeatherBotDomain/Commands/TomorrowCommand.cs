using Newtonsoft.Json;

namespace WeatherBotDomain.Commands
{
    public class TomorrowCommand : WeatherCommand
    {
        public TomorrowCommand(HttpClient client, WeatherCore domain, string uri) 
            : base(client, domain, uri)
        {

        }

        public override string Description => "Погода и температура завтра";

        protected override WeatherReply ProcessResponse(string jsonResponse)
        {
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(jsonResponse);

            var utcOffset = parsedJson.UtcOffsetSeconds;
            var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

            var temperatures = parsedJson.Data.TemperaturePoints.Skip(24).ToArray();
            var weatherCodes = parsedJson.Data.WeatherCodes.Skip(24).ToArray();

            return weatherDomain.GetReply("Завтра", timeNow, weatherCodes, temperatures);
        }
    }
}
