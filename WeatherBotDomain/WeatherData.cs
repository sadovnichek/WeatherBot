using Newtonsoft.Json;

namespace WeatherBotDomain
{
    public class WeatherData
    {
        [JsonProperty("temperature_2m")]
        public double[] TemperaturePoints { get; set; }

        [JsonProperty("weather_code")]
        public int[] WeatherCodes { get; set; }
    }
}
