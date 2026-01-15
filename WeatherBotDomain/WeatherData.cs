using Newtonsoft.Json;

namespace BotInfrastructure
{
    public class WeatherData
    {
        [JsonProperty("temperature_2m")]
        public double[] TemperaturePoints { get; set; }

        [JsonProperty("weather_code")]
        public int[] WeatherCodes { get; set; }
    }
}
