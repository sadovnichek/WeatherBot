using Newtonsoft.Json;
using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain
{
    public class WeatherDTO
    {
        public DateTime Now { get; init; }

        public TimeOnly Sunrise { get; init; }

        public TimeOnly Sunset { get; init; }

        public double[] Temperatures { get; init; }

        public int[] WeatherCodes { get; init; }
    }

    public interface IWeatherApiController
    {
        Task<WeatherDTO> SendRequest();
    }

    public class OpenMeteoController : IWeatherApiController
    {
        public readonly HttpClient _client;

        public OpenMeteoController(HttpClient client)
        {
            _client = client;
        }

        public async Task<WeatherDTO> SendRequest()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress);
            httpRequest.Content = GetValues();

            var response = await _client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(responseContent);

                var sunrise = TimeOnly.Parse(parsedJson.DailyData.Sunrise[0]);
                var sunset = TimeOnly.Parse(parsedJson.DailyData.Sunset[0]);

                return new WeatherDTO()
                {
                    Sunrise = sunrise,
                    Sunset = sunset,
                    Temperatures = parsedJson.WeatherData.TemperaturePoints,
                    WeatherCodes = parsedJson.WeatherData.WeatherCodes,
                    Now = DateTime.UtcNow.AddSeconds(parsedJson.UtcOffsetSeconds)
                };
            }
            catch (JsonException)
            {
                // log
                throw;
            }
        }

        private HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", "56.823457" },
                  { "longitude", "60.551424" },
                  { "daily", "sunrise,sunset" },
                  { "hourly", "temperature_2m,weathercode" },
                  { "timezone", "auto" },
                  { "forecast_days", "1" }
            };
            return new FormUrlEncodedContent(values);
        }
    }
}