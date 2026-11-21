using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class WeeklyCommand : ICommand
    {
        private readonly IWeatherApiClient _client;
        private readonly IWeatherApiController _controller;
        private readonly IWeatherService _weatherService;
        private readonly ICoordinateResolver _coordinateResolver;
        private readonly IUserService _userService;

        public string Description => "прогноз погоды на неделю";

        public string Name => "/weekly";

        public WeeklyCommand(IWeatherApiClient client,
            IWeatherApiController controller,
            IWeatherService weatherService,
            ICoordinateResolver coordinateResolver,
            IUserService userService)
        {
            _client = client;
            _controller = controller;
            _weatherService = weatherService;
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
                ForecastDays = 7
            };

            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError();

            var aggregatedData = AggregateData(dto).ToList();

            return new WeeklyReply(aggregatedData);
        }

        private IEnumerable<DailyWeatherData> AggregateData(WeatherInfo dto)
        {
            var weatherCodes = dto.WeatherCodes
                .Chunk(24)
                .Select(day =>
                {
                    return day
                        .Mode(1)
                        .Concat(day.Where(code => _weatherService.IsPrecipitation(code)))
                        .Select(x => _weatherService.GetEmoji(x))
                        .Distinct();
                })
                .ToArray();

            var dates = dto.ForecastHours
                .Chunk(24)
                .Select(d =>
                {
                    var firstHour = d.First();
                    return new DateOnly(firstHour.Year, firstHour.Month, firstHour.Day);
                })
                .ToArray();

            var medianTemperature = dto.Temperatures
                .Chunk(24)
                .Select(d => d.Median())
                .ToArray();
            
            for (var i = 0; i < weatherCodes.Length; i++)
            {
                yield return new DailyWeatherData(dates[i], weatherCodes[i], medianTemperature[i]);
            }
        }
    }
}