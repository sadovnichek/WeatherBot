using Newtonsoft.Json;

namespace WeatherBotDomain.OpenMeteo
{
    public class Hourly
    {
        [JsonProperty("temperature_2m")]
        public double[] TemperaturePoints { get; set; }

        [JsonProperty("weather_code")]
        public int[] WeatherCodes { get; set; }
    }
}
