using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class HourlyCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly IWeatherService _weatherService;
        private readonly ICoordinateResolver _coordinateResolver;
        private readonly IUserService _userService;

        public string Description => "Погода и температура на каждый час сегодня";

        public string Name => "/hourly";

        public HourlyCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            IWeatherService weatherService,
            ICoordinateResolver coordinateResolver,
            IUserService userService)
        {
            _controller = controller;
            _weatherService = weatherService;
            _client = client;
            _coordinateResolver = coordinateResolver;
            _userService = userService;
        }

        public async Task<Reply?> Execute(UserInfo userInfo, string[] args)
        {
            _userService.CreateIfNotExists(userInfo.ChatId);
            var user = _userService.UpdateUserLastQueryTime(userInfo.ChatId, userInfo.LastMessageTimeUtc);

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
                return PlainReply.OnError(); ;

            var startIndex = args.Length > 0 ? int.Parse(args[0]) : 0;
            var endIndex = args.Length > 0 ? int.Parse(args[1]) + 1 : 24;

            var reply = new HourlyForecastReply();
            var daytime = new TimeSegment(dto.Sunrise, dto.Sunset);
            for(var i = startIndex; i < endIndex; i++)
            {
                var time = new TimeOnly(i, 0);
                var isDay = daytime.IsTimeInSegment(time);
                var emoji = _weatherService.GetEmoji(dto.WeatherCodes[i], !isDay);
                var temperature = dto.Temperatures[i];
                var data = new HourlyForecastData(time, emoji, temperature);
                reply.AppendData(data);
            }

            return reply;
        }
    }
}