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
        
        protected override IReply GetPrecipitationForecast(WeatherDTO dto)
        {
            var weatherCodes = dto.WeatherCodes.Skip(24).Take(24).ToArray();
            return _domain.GetPrecipitationForecast(weatherCodes);
        }

        protected override IReply ProcessResponse(WeatherDTO dto)
        {
            var temperatures = dto.Temperatures.Skip(24).ToArray();
            var weatherCodes = dto.WeatherCodes.Skip(24).ToArray();

            return _domain.GetReply("Завтра", dto.Now, weatherCodes, temperatures);
        }
    }
}