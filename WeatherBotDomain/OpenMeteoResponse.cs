using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBotDomain.Commands;

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
