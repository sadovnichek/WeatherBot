using BotInfrastructure;

namespace WeatherBotDomain.Replies
{
    public abstract class WeatherReply : Reply
    {
        public required string TimePointer { get; init; }

        public DateTime Date { get; init; }

        public string Greeting { get; init; }

        public required bool IsGreetingNeeded { get; init; }

        public string Emoji { get; init; }

        public double MedianTemperature { get; init; }

        public double MinTemperature { get; init; }

        public double MaxTemperature { get; init; }

        public abstract override string BuildMessage();
    }
}
