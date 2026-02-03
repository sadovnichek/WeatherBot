using System.Threading.Channels;

namespace WeatherBotDomain.Commands
{
    public class TomorrowCommand : WeatherCommand
    {
        public TomorrowCommand(HttpClient client, WeatherCore domain, ChannelWriter<string> bus, string uri)
            : base(client, domain, bus, uri)
        {

        }

        public override string Description => "Погода, температура и осадки на завтра";
        
        protected override IReply GetPrecipitationForecast(OpenMeteoResponse response)
        {
            var weatherCodes = response.Data.WeatherCodes.Skip(24).Take(24).ToArray();
            return weatherDomain.GetPrecipitationForecast(weatherCodes);
        }

        protected override IReply ProcessResponse(OpenMeteoResponse response)
        {
            var utcOffset = response.UtcOffsetSeconds;
            var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

            var temperatures = response.Data.TemperaturePoints.Skip(24).ToArray();
            var weatherCodes = response.Data.WeatherCodes.Skip(24).ToArray();

            return weatherDomain.GetReply("Завтра", timeNow, weatherCodes, temperatures);
        }
    }
}