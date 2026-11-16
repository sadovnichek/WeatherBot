using System.Globalization;
using WeatherBotDomain.Abstractions;

namespace WeatherBotDomain.OpenMeteo
{
    public class OpenMeteoWeatherRequest : WeatherRequest
    {
        public override HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", Coordinates.Latitude.ToString(CultureInfo.InvariantCulture) },
                  { "longitude", Coordinates.Longitude.ToString(CultureInfo.InvariantCulture) },
                  { "daily", "sunrice,suncet" },
                  { "hourly", "temperature_2m,weather_code" },
                  { "timezone", "auto" },
                  { "forecast_days", ForecastDays.ToString() }
            };
            return new FormUrlEncodedContent(values);
        }
    }
}