namespace WeatherBotDomain.Entities
{
    public record DailyWeatherData(DateOnly Date, IEnumerable<string> Weather, double MedianTemperature);
}