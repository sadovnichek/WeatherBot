using BotInfrastructure;

namespace WeatherBotDomain.Commands
{
    public class TodayCommand : WeatherCommand
    {
        public TodayCommand(IWeatherApiController controller,
            WeatherCore domain)
            : base(controller, domain)
        {

        }

        public override string Description => "Погода, температура и осадки сегодня";

        protected override IReply GetPrecipitationForecast(WeatherDTO dto)
        {
            var weatherCodes = dto.WeatherCodes.Take(24).ToArray();
            return _domain.GetPrecipitationForecast(weatherCodes);
        }

        protected override IReply ProcessResponse(WeatherDTO dto)
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