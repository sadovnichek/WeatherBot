using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(IWeatherApiController controller,
            IWeatherApiClient client,
            IWeatherService weatherService,
            IPrecipitationService precipitationService,
            ICoordinateResolver coordinateResolver,
            IUserService userService)
            : base(controller, client, weatherService, precipitationService, coordinateResolver, userService)
        {

        }

        public override string Description => "Погода, температура и осадки сегодня";

        public override string Name => "/today";

        protected override Reply GetPrecipitationForecast(WeatherInfo dto)
        {
            var weatherCodes = dto.WeatherCodes.Take(24).ToArray();
            return _precipitationService.GetPrecipitationForecast(weatherCodes);
        }

        protected override Reply ProcessResponse(WeatherInfo dto)
        {
            var temperatures = dto.Temperatures.Take(24).ToArray();
            var sunriseHour = dto.Sunrise.Hour;
            var sunsetHour = dto.Sunset.Hour;
            var weatherCodes = dto.WeatherCodes.Skip(sunriseHour + 1)
                .Take(sunsetHour - sunriseHour)
                .ToArray();

            return _weatherService.GetReply("Сегодня", dto.Now, weatherCodes, temperatures);
        }
    }
}