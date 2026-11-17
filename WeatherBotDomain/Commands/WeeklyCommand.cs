using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class WeeklyCommand : CitySpecificCommand
    {
        private readonly IWeatherApiClient _client;
        private readonly IWeatherApiController _controller;
        private readonly IWeatherService _weatherService;

        public override string Description => "прогноз погоды на неделю";

        public override string Name => "/weekly";

        public WeeklyCommand(IWeatherApiClient client,
            IWeatherApiController controller,
            IWeatherService weatherService,
            ICityResolverService cityResolverService) : base(cityResolverService)
        {
            _client = client;
            _controller = controller;
            _weatherService = weatherService;
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