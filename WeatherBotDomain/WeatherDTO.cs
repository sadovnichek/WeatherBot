namespace WeatherBotDomain
{
    public class WeatherDTO
    {
        public DateTime Now { get; init; }

        public TimeOnly Sunrise { get; init; }

        public TimeOnly Sunset { get; init; }

        public double[] Temperatures { get; init; }

        public int[] WeatherCodes { get; init; }
    }
}