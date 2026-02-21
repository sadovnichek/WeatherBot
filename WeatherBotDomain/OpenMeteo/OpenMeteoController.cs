using Newtonsoft.Json;
using System.Globalization;

namespace WeatherBotDomain.OpenMeteo
{
    public class OpenMeteoController : IWeatherApiController
    {
        public readonly HttpClient _client;

        public OpenMeteoController(HttpClient client)
        {
            _client = client;
        }

        /// <exception cref="JsonException"></exception>
        public async Task<WeatherDTO?> TrySendRequest()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress);
            var content = GetValues();
            httpRequest.Content = content;

            var response = await _client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(responseContent);

                var sunrise = TimeOnly.Parse(parsedJson.DailyData.Sunrise[0]);
                var sunset = TimeOnly.Parse(parsedJson.DailyData.Sunset[0]);
                var now = DateTime.UtcNow.AddSeconds(parsedJson.UtcOffsetSeconds);

                return new WeatherDTO()
                {
                    Sunrise = sunrise,
                    Sunset = sunset,
                    Temperatures = parsedJson.WeatherData.TemperaturePoints,
                    WeatherCodes = parsedJson.WeatherData.WeatherCodes,
                    Now = now
                };
            }
            catch (JsonException)
            {
                Console.WriteLine("A problem occured while parsing JSON reply from server");
                Console.WriteLine("The request was: ");
                Console.WriteLine(await content.ReadAsStringAsync());
                return default;
            }
        }

        private HttpContent GetValues(double latitude = 56.82, double longitude = 60.55)
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", latitude.ToString(CultureInfo.InvariantCulture) },
                  { "longitude", longitude.ToString(CultureInfo.InvariantCulture) },
                  { "daily", "sunrise,sunset" },
                  { "hourly", "temperature_2m,weather_code" },
                  { "timezone", "auto" },
                  { "forecast_days", "2" }
            };
            return new FormUrlEncodedContent(values);
        }
    }
}