using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        protected readonly WeatherCore _domain;

        public abstract string Description { get; }

        public WeatherCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            WeatherCore domain)
        {
            _controller = controller;
            _domain = domain;
            _client = client;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            var request = new OpenMeteoWeatherRequest() 
            { 
                Latitude = 56.82, 
                Longitude = 60.55,
                ForecastDays = 2
            };
            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return null;

            return ProcessResponse(dto)
                .FollowWith(GetPrecipitationForecast(dto));
        }

        protected abstract Reply ProcessResponse(WeatherInfo dto);

        protected abstract Reply GetPrecipitationForecast(WeatherInfo dto);
    }
}