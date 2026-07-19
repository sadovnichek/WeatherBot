using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain.OpenMeteo
{
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
}