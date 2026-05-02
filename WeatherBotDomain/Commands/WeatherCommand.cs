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

        public async Task<Reply?> Execute(string[] args)
        {
            var dto = await _controller.TrySendRequest();

            if (dto is null)
                return null;

            return ProcessResponse(dto)
                .FollowWith(GetPrecipitationForecast(dto));
        }

        protected abstract Reply ProcessResponse(WeatherApiResponse dto);

        protected abstract Reply GetPrecipitationForecast(WeatherApiResponse dto);
    }
}