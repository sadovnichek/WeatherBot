using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly CoordinateArgumentResolver _coordinateArgumentResolver;
        protected readonly WeatherCore _domain;

        public abstract string Description { get; }

        public WeatherCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            WeatherCore domain,
            CoordinateArgumentResolver coordinateArgumentResolver)
        {
            _controller = controller;
            _domain = domain;
            _client = client;
            _coordinateArgumentResolver = coordinateArgumentResolver;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            if (!_coordinateArgumentResolver.TryGetCoordinates(args, out var coordinates, out var message))
            {
                return new PlainReply(message);
            }

            var request = new OpenMeteoWeatherRequest() 
            { 
                Coordinates = coordinates,
                ForecastDays = 2
            };

            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError();

            return ProcessResponse(dto)
                .FollowWith(GetPrecipitationForecast(dto));
        }

        protected abstract Reply ProcessResponse(WeatherInfo dto);

        protected abstract Reply GetPrecipitationForecast(WeatherInfo dto);
    }
}