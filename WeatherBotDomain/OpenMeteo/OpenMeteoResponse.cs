using Newtonsoft.Json;

namespace WeatherBotDomain.OpenMeteo
{
    public class DailyData
    {
        [JsonProperty("sunrise")]
        public string[] Sunrise { get; set; }

        [JsonProperty("sunset")]
        public string[] Sunset { get; set; }
    }

    public class OpenMeteoResponse
    {
        [JsonProperty("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonProperty("hourly")]
        public WeatherData WeatherData { get; set; }

        [JsonProperty("daily")]
        public DailyData DailyData { get; set; }
    }
}
