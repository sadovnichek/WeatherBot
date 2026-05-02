using BotInfrastructure;

namespace WeatherBotDomain.Commands
{
    public class TomorrowCommand : WeatherCommand
    {
        public TomorrowCommand(IWeatherApiController controller, 
            WeatherCore domain)
            : base(controller, domain)
        {

        }

        public override string Description => "Погода, температура и осадки на завтра";
        
        protected override Reply GetPrecipitationForecast(WeatherApiResponse dto)
        {
            var weatherCodes = dto.WeatherCodes.Skip(24).Take(24).ToArray();
            return _domain.GetPrecipitationForecast(weatherCodes);
        }

        protected override Reply ProcessResponse(WeatherApiResponse dto)
        {
            var temperatures = dto.Temperatures.Skip(24).ToArray();
            var sunriseHour = dto.Sunrise.Hour;
            var sunsetHour = dto.Sunset.Hour;
            var weatherCodes = dto.WeatherCodes.Skip(24 + sunriseHour + 1)
                .Take(sunsetHour - sunriseHour)
                .ToArray();

            return _domain.GetReply("Завтра", dto.Now, weatherCodes, temperatures);
        }
    }
}