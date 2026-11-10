using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(IWeatherApiController controller,
            IWeatherApiClient client,
            WeatherCore domain,
            ICityResolverService cityResolverService)
            : base(controller, client, domain, cityResolverService)
        {

        }

        public override string Description => "Погода, температура и осадки сегодня";

        public override string Name => "/today";

        protected override Reply GetPrecipitationForecast(WeatherInfo dto)
        {
            var weatherCodes = dto.WeatherCodes.Take(24).ToArray();
            return _domain.GetPrecipitationForecast(weatherCodes);
        }

        protected override Reply ProcessResponse(WeatherInfo dto)
        {
            var temperatures = dto.Temperatures.Take(24).ToArray();
            var sunriseHour = dto.Sunrise.Hour;
            var sunsetHour = dto.Sunset.Hour;
            var weatherCodes = dto.WeatherCodes.Skip(sunriseHour + 1)
                .Take(sunsetHour - sunriseHour)
                .ToArray();

            return _domain.GetReply("Сегодня", dto.Now, weatherCodes, temperatures);
        }
    }
}