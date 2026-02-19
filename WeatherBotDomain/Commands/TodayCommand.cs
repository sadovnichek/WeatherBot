using BotInfrastructure;
using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(HttpClient client, 
            WeatherCore domain,
            string uri) 
            : base(client, domain, uri)
        {

        }

        public override string Description => "Погода, температура и осадки сегодня";

        protected override IReply GetPrecipitationForecast(OpenMeteoResponse response)
        {
            var weatherCodes = response.WeatherData.WeatherCodes.Take(24).ToArray();
            return weatherDomain.GetPrecipitationForecast(weatherCodes);
        }

        protected override IReply ProcessResponse(OpenMeteoResponse response)
        {
            var utcOffset = response.UtcOffsetSeconds;
            var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

            var temperatures = response.WeatherData.TemperaturePoints.Take(24).ToArray();
            var weatherCodes = response.WeatherData.WeatherCodes.Take(24).ToArray();

            return weatherDomain.GetReply("Сегодня", timeNow, weatherCodes, temperatures);
        }
    }
}
