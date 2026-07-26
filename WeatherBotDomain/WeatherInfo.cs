namespace WeatherBotDomain
{
    public class WeatherInfo
    {
        public DateTime Now { get; init; }

        public DateTime[] ForecastHours { get; init; }

        public TimeOnly Sunrise { get; init; }

        public TimeOnly Sunset { get; init; }

        public double[] Temperatures { get; init; }

        public int[] WeatherCodes { get; init; }
    }
}