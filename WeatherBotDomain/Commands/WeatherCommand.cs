using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : CitySpecificCommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        protected readonly IWeatherService _weatherService;
        protected readonly IPrecipitationService _precipitationService;

        public override abstract string Description { get; }

        public override abstract string Name { get; }

        public WeatherCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            IWeatherService weatherService,
            ICityResolverService cityResolverService,
            IPrecipitationService precipitationService) : base(cityResolverService)
        {
            _controller = controller;
            _weatherService = weatherService;
            _client = client;
            _precipitationService = precipitationService;
        }

        public override async Task<Reply?> Execute(string[] args)
        {
            if (!TryResolveCoordinates(args, out var message, out var coordinates))
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