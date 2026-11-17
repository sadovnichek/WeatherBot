using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : CitySpecificCommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;

        public override string Description => "Время заката и рассвета сегодня";

        public override string Name => "/daytime";

        public DaytimeCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            ICityResolverService cityResolverService) : base(cityResolverService)
        {
            _controller = controller;
            _client = client;
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