using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;

namespace WeatherBotDomain.OpenMeteo
{
    public class OpenMeteoController : IWeatherApiController
    {
        private readonly ILogger<OpenMeteoController> _logger;

        public OpenMeteoController(ILogger<OpenMeteoController> logger)
        {
            _logger = logger;
        } 

        public WeatherInfo? TryProcessApiResponse(string json)
        {
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(json);

            if (parsedJson == null)
            {
                _logger.LogError("Deserialization error\n{json}", json);
                return default;
            }

            if (parsedJson.Error)
            {
                _logger.LogError(parsedJson.ErrorReason);
                return default;
            }

            var sunrise = TimeOnly.Parse(parsedJson.DailyData.Sunrise[0]);
            var sunset = TimeOnly.Parse(parsedJson.DailyData.Sunset[0]);
            var now = DateTime.UtcNow.AddSeconds(parsedJson.UtcOffsetSeconds);
            var forecastHours = parsedJson.Hourly.Timestamps.Select(DateTime.Parse).ToArray();

            return new WeatherInfo()
            {
                Sunrise = sunrise,
                Sunset = sunset,
                Temperatures = parsedJson.Hourly.TemperaturePoints,
                WeatherCodes = parsedJson.Hourly.WeatherCodes,
                Now = now,
                ForecastHours = forecastHours
            };
        }
    }
}