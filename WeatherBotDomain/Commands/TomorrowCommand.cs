using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class TomorrowCommand : WeatherCommand
    {
        public TomorrowCommand(IWeatherApiController controller,
            IWeatherApiClient client,
            WeatherCore domain,
            ICityResolverService cityResolverService)
            : base(controller, client, domain, cityResolverService)
        {

        }

        public override string Description => "Погода, температура и осадки на завтра";

        public override string Name => "/tomorrow";
        protected override Reply GetPrecipitationForecast(WeatherInfo dto)
        {
            var weatherCodes = dto.WeatherCodes.Skip(24).Take(24).ToArray();
            return _domain.GetPrecipitationForecast(weatherCodes);
        }

        protected override Reply ProcessResponse(WeatherInfo dto)
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