using Newtonsoft.Json;
using System.Globalization;

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

                return new WeatherInfo()
                {
                    Sunrise = sunrise,
                    Sunset = sunset,
                    Temperatures = parsedJson.WeatherData.TemperaturePoints,
                    WeatherCodes = parsedJson.WeatherData.WeatherCodes,
                    Now = now
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