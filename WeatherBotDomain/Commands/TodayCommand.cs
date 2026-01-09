using Newtonsoft.Json;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(HttpClient client, WeatherCore domain, string uri) 
            : base(client, domain, uri)
        {

        }

        protected override WeatherReply ProcessResponse(string jsonResponse)
        {
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(jsonResponse);

            var utcOffset = parsedJson.UtcOffsetSeconds;
            var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

            var temperatures = parsedJson.Data.TemperaturePoints.Take(24).ToArray();
            var weatherCodes = parsedJson.Data.WeatherCodes.Take(24).ToArray();

            return GetMessage("Сегодня", timeNow, weatherCodes, temperatures);
        }
    }
}
