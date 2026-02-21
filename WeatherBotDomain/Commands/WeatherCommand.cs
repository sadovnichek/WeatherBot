using BotInfrastructure;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly IWeatherApiController _controller;

        protected readonly WeatherCore _domain;

        public abstract string Description { get; }

        public WeatherCommand(IWeatherApiController controller, 
            WeatherCore domain)
        {
            _controller = controller;
            _domain = domain;
        }

        public async IAsyncEnumerable<IReply> Execute(string[] args)
        {
            var dto = await _controller.TrySendRequest();

            if (dto is null)
                yield break;

            yield return ProcessResponse(dto);
            yield return GetPrecipitationForecast(dto);
        }

        protected abstract IReply ProcessResponse(WeatherDTO dto);

        protected abstract IReply GetPrecipitationForecast(WeatherDTO dto);
    }
}