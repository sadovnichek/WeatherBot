using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public record DailyWeatherData(DateOnly Date, IEnumerable<string> Weather, double MedianTemperature);

    public class WeeklyCommand : ICommand
    {
        private readonly IWeatherApiClient _client;
        private readonly IWeatherApiController _controller;
        private readonly WeatherCore _domain;
        private readonly ICityResolverService _cityResolverService;

        public string Description => "прогноз погоды на неделю";

        public WeeklyCommand(IWeatherApiClient client,
            IWeatherApiController controller,
            WeatherCore domain,
            ICityResolverService cityResolverService)
        {
            _client = client;
            _controller = controller;
            _domain = domain;
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
                        .Concat(day.Where(code => _domain.IsPrecipitation(code)))
                        .Select(x => _domain.GetEmoji(x))
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