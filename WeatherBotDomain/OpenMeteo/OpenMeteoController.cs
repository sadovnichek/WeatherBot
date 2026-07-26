using Newtonsoft.Json;
using WeatherBotDomain.Abstractions;

namespace WeatherBotDomain.OpenMeteo
{
    public class OpenMeteoController : IWeatherApiController
    {
        public WeatherInfo? TryProcessApiResponse(string json)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(json);

                if (parsedJson == null || parsedJson.Error)
                {
                    Console.WriteLine(parsedJson?.ErrorReason);
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
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
                return default;
            }
        }
    }
}