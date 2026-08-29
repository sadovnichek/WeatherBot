using WeatherBotDomain.Services;

namespace WeatherBotDomain.Abstractions
{
    public abstract class WeatherRequest
    {
        public Coordinates Coordinates { get; init; }

        public int ForecastDays { get; init; }

        public abstract HttpContent GetValues();
    }
}
