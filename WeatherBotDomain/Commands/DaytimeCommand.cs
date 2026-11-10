using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly ICityResolverService _cityResolverService;

        public string Description => "Время заката и рассвета сегодня";

        public string Name => "/daytime";

        public DaytimeCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            ICityResolverService cityResolverService)
        {
            _controller = controller;
            _client = client;
            _cityResolverService = cityResolverService;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            var resolveResult = _cityResolverService.TryResolve(args[0]);
            if (!resolveResult.Success)
                return new PlainReply("Не удалось определить город");

            var request = new OpenMeteoWeatherRequest() 
            { 
                Coordinates = resolveResult.Value,
                ForecastDays = 1
            };

            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError();

            var segment = new TimeSegment(dto.Sunrise, dto.Sunset);

            return new DaytimeReply(segment);
        }
    }
}