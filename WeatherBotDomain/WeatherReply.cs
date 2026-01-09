namespace WeatherBotDomain
{
    public abstract class WeatherReply
    {
        public string TimePointer { get; init; }

        public string Greeting { get; init; }

        public string Weather { get; init; }

        public string Wording { get; init; }

        public string Emoji { get; init; }

        public double MedianTemperature { get; init; }

        public double MinTemperature { get; init; }

        public double MaxTemperature { get; init; }

        public abstract string BuildMessage();
    }
}
