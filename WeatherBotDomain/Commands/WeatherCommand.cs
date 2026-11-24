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
        protected readonly IWeatherService _weatherService;
        protected readonly IPrecipitationService _precipitationService;
        private readonly ICoordinateResolver _coordinateResolver;
        private readonly IUserService _userService;

        private const int _timeToRememberInSeconds = 60 * 10;

        public abstract string Description { get; }

        public  abstract string Name { get; }

        public WeatherCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            IWeatherService weatherService,
            IPrecipitationService precipitationService,
            ICoordinateResolver coordinateResolver,
            IUserService userService)
        {
            _controller = controller;
            _weatherService = weatherService;
            _client = client;
            _precipitationService = precipitationService;
            _coordinateResolver = coordinateResolver;
            _userService = userService;
        }

        public async Task<Reply?> Execute(UserInfo userInfo, string[] args)
        {
            _userService.CreateIfNotExists(userInfo.ChatId);
            var lastQueryTime = _userService.GetUserByChatId(userInfo.ChatId).LastQueryTime;
            var user = _userService.UpdateUserLastQueryTime(userInfo.ChatId, userInfo.LastMessageTimeUtc);

            if (!_coordinateResolver.TryResolveCoordinates(user, args, out var message, out var coordinates))
            {
                return new PlainReply(message);
            }

            var needToGreeting = user.LastQueryTime - lastQueryTime > _timeToRememberInSeconds;

            var request = new OpenMeteoWeatherRequest() 
            { 
                Coordinates = coordinates,
                ForecastDays = 2
            };

            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError();

            return ProcessResponse(dto, needToGreeting)
                .FollowWith(GetPrecipitationForecast(dto));
        }

        protected abstract Reply ProcessResponse(WeatherInfo dto, bool isGreetingNeeded);

        protected abstract Reply GetPrecipitationForecast(WeatherInfo dto);
    }
}