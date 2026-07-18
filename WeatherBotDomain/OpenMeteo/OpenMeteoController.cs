using Newtonsoft.Json;
using System.Globalization;

namespace WeatherBotDomain.OpenMeteo
{
    public abstract class WeatherRequest
    {
        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public abstract HttpContent GetValues();
    }

    public class OpenMeteoWeatherRequest : WeatherRequest
    {
        public override HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", Latitude.ToString(CultureInfo.InvariantCulture) },
                  { "longitude", Longitude.ToString(CultureInfo.InvariantCulture) },
                  { "daily", "sunrise,sunset" },
                  { "hourly", "temperature_2m,weather_code" },
                  { "timezone", "auto" },
                  { "forecast_days", "2" }
            };
            return new FormUrlEncodedContent(values);
        }
    }

    public class OpenMeteoController : IWeatherApiController
    {
        public WeatherApiResponse? TryProcessApiResponse(string json)
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

                return new WeatherApiResponse()
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

    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _client;

        public WeatherApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> TrySendRequestAsync(WeatherRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress);
            httpRequest.Content = request.GetValues();
            var response = await _client.SendAsync(httpRequest);
            return await response.Content.ReadAsStringAsync();
        }
    }
}