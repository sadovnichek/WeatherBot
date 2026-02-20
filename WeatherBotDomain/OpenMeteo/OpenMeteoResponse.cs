using Newtonsoft.Json;

namespace WeatherBotDomain.OpenMeteo
{
    public class Daily
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
        public Hourly WeatherData { get; set; }

        [JsonProperty("daily")]
        public Daily DailyData { get; set; }
    }
}
