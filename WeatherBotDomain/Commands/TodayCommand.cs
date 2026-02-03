using BotInfrastructure;
using System.Text;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(HttpClient client, WeatherCore domain, IMessageBus<string> bus, string uri) 
            : base(client, domain, bus, uri)
        {

        }

        public override string Description => "Погода, температура и осадки сегодня";

        protected override string GetPrecipitationForecast(OpenMeteoResponse response)
        {
            var weatherCodes = response.Data.WeatherCodes.Take(24).ToArray();
            return weatherDomain.GetPrecipitationForecast(weatherCodes).BuildMessage();
        }

        protected override WeatherReply ProcessResponse(OpenMeteoResponse response)
        {
            var utcOffset = response.UtcOffsetSeconds;
            var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

            var temperatures = response.Data.TemperaturePoints.Take(24).ToArray();
            var weatherCodes = response.Data.WeatherCodes.Take(24).ToArray();

            return weatherDomain.GetReply("Сегодня", timeNow, weatherCodes, temperatures);
        }
    }
}
