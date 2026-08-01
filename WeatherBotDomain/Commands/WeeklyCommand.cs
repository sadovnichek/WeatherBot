using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Commands
{
    public record DailyWeatherData(DateOnly Date, IEnumerable<string> Weather, double MedianTemperature);

    public class WeeklyCommand : ICommand
    {
        private readonly IWeatherApiClient _client;
        private readonly IWeatherApiController _controller;
        private readonly WeatherCore _domain;

        public string Description => "прогноз погоды на неделю";

        public WeeklyCommand(IWeatherApiClient client,
            IWeatherApiController controller,
            WeatherCore domain)
        {
            _client = client;
            _controller = controller;
            _domain = domain;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            if (!AreArgumentsValid(args))
                return new PlainReply("Неверные аргументы");

            var request = new OpenMeteoWeatherRequest() 
            { 
                //Latitude = 56.82, 
                //Longitude = 60.55,
                Latitude = double.Parse(args[0]),
                Longitude = double.Parse(args[1]),
                ForecastDays = 7
            };

            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError();

            var aggregatedData = AggregateData(dto).ToList();

            return new WeeklyReply(aggregatedData);
        }

        public bool AreArgumentsValid(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }

            return args.Length == 2
                && double.TryParse(args[0], out _)
                && double.TryParse(args[1], out _);
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