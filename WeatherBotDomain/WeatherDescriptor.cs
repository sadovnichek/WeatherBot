namespace WeatherBotDomain
{
    public class WeatherDescriptor(string description,
        string dayEmoji,
        string nightEmoji,
        bool isWordingNeeded,
        bool isPrecipitation)
    {
        public string Description { get; init; } = description;

        public string DayEmoji { get; init; } = dayEmoji;

        public string NightEmoji { get; init; } = nightEmoji;

        public bool IsWordingNeeded { get; init; } = isWordingNeeded;

        public bool IsPrecipitation { get; init; } = isPrecipitation;
    }
}