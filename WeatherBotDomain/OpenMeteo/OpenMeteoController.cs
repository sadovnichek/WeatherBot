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
        public readonly HttpClient _client;

        public OpenMeteoController(HttpClient client)
        {
            _client = client;
        }

        public async Task<WeatherReply?> TrySendRequest()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress);
            var request = new OpenMeteoWeatherRequest() { Latitude = 56.82, Longitude = 60.55 };
            var content = request.GetValues();
            httpRequest.Content = content;

            var response = await _client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            try
            {
                var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(responseContent);

                var sunrise = TimeOnly.Parse(parsedJson.DailyData.Sunrise[0]);
                var sunset = TimeOnly.Parse(parsedJson.DailyData.Sunset[0]);
                var now = DateTime.UtcNow.AddSeconds(parsedJson.UtcOffsetSeconds);

                return new WeatherReply()
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
    }
}