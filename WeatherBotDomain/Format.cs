namespace WeatherBotDomain
{
    public static class Format
    {
        public static string Temperature(double temperature)
        {
            return $"{Math.Round(temperature, 1)}°C";
        }
    }
}