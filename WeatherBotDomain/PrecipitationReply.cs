using System.Text;

namespace WeatherBotDomain
{
    public interface IReply
    {
        string BuildMessage();
    }

    public record TimeSegment(TimeOnly Start, TimeOnly End)
    {
        public string GetStringRepresentation()
        {
            return $"{Start} - {End}";
        }
    }

    public record WeatherSegment(string Description, string Emoji, TimeSegment[] TimeSegments);

    public record PrecipitationReply(IEnumerable<WeatherSegment> WeatherSegments) : IReply
    {
        public string BuildMessage()
        {
            if (!WeatherSegments.Any())
                return "Осадки не ожидаются";

            var builder = new StringBuilder();
            builder.Append("Ожидаются осадки:\n");

            foreach (var item in WeatherSegments)
            {
                builder.Append(item.Description);
                builder.Append(' ');
                builder.Append(item.Emoji);
                builder.Append(' ');
                builder.Append(string.Join(", ", item.TimeSegments.Select(segment => segment.GetStringRepresentation())));
                builder.Append('\n');
            }

            return builder.ToString();
        }
    }
}