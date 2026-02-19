using BotInfrastructure;
using Newtonsoft.Json;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Reply;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly string uriAddress;
        private readonly IWeatherApiController _controller;

        public string Description => "Время заката и рассвета сегодня";

        public DaytimeCommand(HttpClient client, 
            string uri,
            IWeatherApiController controller)
        {
            httpClient = client;
            uriAddress = uri;
            _controller = controller;
        }

        public async IAsyncEnumerable<IReply> Execute(string[] args)
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var content = await response.Content.ReadAsStringAsync();
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(content);
            var sunrise = parsedJson.DailyData.Sunrise[0];
            var sunset = parsedJson.DailyData.Sunset[0];

            var output = await _controller.SendRequest();
            var srise = output.Sunrise;
            var sset = output.Sunset;
            var seg = new TimeSegment(srise, sset);
            var repl = new DaytimeReply(seg);

            yield return GetDaytimeReply(sunrise, sunset);
        }

        public IReply GetDaytimeReply(string sunriseTime, string sunsetTime)
        {
            var timeSegment = GetDayTime(sunriseTime, sunsetTime);
            return new DaytimeReply(timeSegment);
        }

        public TimeSegment GetDayTime(string sunriseTime, string sunsetTime)
        {
            var start = TimeOnly.Parse(sunriseTime);
            var end = TimeOnly.Parse(sunsetTime);
            return new TimeSegment(start, end);
        }

        private HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", "56.823457" },
                  { "longitude", "60.551424" },
                  { "daily", "sunrise,sunset" },
                  { "timezone", "auto" },
                  { "forecast_days", "1" }
            };
            return new FormUrlEncodedContent(values);
        }
    }
}