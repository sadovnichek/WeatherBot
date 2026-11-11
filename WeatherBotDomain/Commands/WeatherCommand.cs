using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly ICityResolverService _cityResolverService;
        protected readonly IWeatherService _weatherService;
        protected readonly IPrecipitationService _precipitationService;

        public abstract string Description { get; }

        public abstract string Name { get; }

        public WeatherCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            IWeatherService weatherService,
            ICityResolverService cityResolverService,
            IPrecipitationService precipitationService)
        {
            _controller = controller;
            _weatherService = weatherService;
            _client = client;
            _cityResolverService = cityResolverService;
            _precipitationService = precipitationService;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            var resolveResult = _cityResolverService.TryResolve(args[0]);
            if (!resolveResult.Success)
                return new PlainReply("Не удалось определить город");

            var request = new OpenMeteoWeatherRequest() 
            { 
                Coordinates = resolveResult.Value,
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