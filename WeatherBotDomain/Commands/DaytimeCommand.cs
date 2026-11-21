using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly ICoordinateResolver _coordinateResolver;
        private readonly IUserService _userService;

        public string Description => "Время заката и рассвета сегодня";

        public string Name => "/daytime";

        public DaytimeCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            ICoordinateResolver coordinateResolver,
            IUserService userService)
        {
            _controller = controller;
            _client = client;
            _coordinateResolver = coordinateResolver;
            _userService = userService;
        }

        public async Task<Reply?> Execute(UserInfo userInfo, string[] args)
        {
            var user = _userService.CreateOrUpdate(userInfo.ChatId, userInfo.LastMessageTimeUtc);

            if (!_coordinateResolver.TryResolveCoordinates(user, args, out var message, out var coordinates))
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