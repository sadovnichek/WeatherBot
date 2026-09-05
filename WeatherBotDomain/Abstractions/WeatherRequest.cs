namespace WeatherBotDomain.Abstractions
{
    public abstract class WeatherRequest
    {
        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public int ForecastDays { get; init; }

        public abstract HttpContent GetValues();
    }
}