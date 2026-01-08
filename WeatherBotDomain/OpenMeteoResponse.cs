using Newtonsoft.Json;

namespace WeatherBotDomain
{
    public class OpenMeteoResponse
    {
        [JsonProperty("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonProperty("hourly")]
        public WeatherData Data { get; set; }
    }
}
