using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly CoordinateArgumentResolver _coordinateArgumentResolver;

        public string Description => "Время заката и рассвета сегодня";

        public DaytimeCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            CoordinateArgumentResolver coordinateArgumentResolver)
        {
            _controller = controller;
            _client = client;
            _coordinateArgumentResolver = coordinateArgumentResolver;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            if (!_coordinateArgumentResolver.TryGetCoordinates(args, out var coordinates, out var message))
                return new PlainReply(message);

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