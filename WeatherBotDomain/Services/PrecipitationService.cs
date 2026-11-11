using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Services
{
    public class PrecipitationService : IPrecipitationService
    {
        private readonly IWeatherService _weatherService;

        public PrecipitationService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public Reply GetPrecipitationForecast(int[] weatherCodes)
        {
            var weatherSegments = GetWeatherSegments(weatherCodes);

            var reply = new PrecipitationReply(weatherSegments);

            return reply;
        }

        private IEnumerable<WeatherSegment> GetWeatherSegments(int[] weatherCodes)
        {
            return weatherCodes.Select(_weatherService.GetWeather)
                    .ToArray()
                    .ClassifyItemsByIndex()
                    .Where(kv => _weatherService.IsPrecipitation(kv.Key))
                    .GroupBy(kv => kv.Key)
                    .Select(group => new WeatherSegment(
                        _weatherService.GetDescription(group.Key),
                        _weatherService.GetEmoji(group.Key),
                        group.Select(p =>
                        {
                            var start = new TimeOnly(p.Value.Item1, 0);
                            var end = new TimeOnly(p.Value.Item2, 0).AddHours(1);
                            return new TimeSegment(start, end);
                        })
                    .ToArray()));
        }
    }
}