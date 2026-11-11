namespace WeatherBotDomain.Entities
{
    public record WeatherSegment(string Description, string Emoji, TimeSegment[] TimeSegments);
}