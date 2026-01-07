using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
