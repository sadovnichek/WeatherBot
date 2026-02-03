namespace WeatherBotDomain
{
    public record WeatherSegment(string Description, string Emoji, TimeSegment[] TimeSegments);
}