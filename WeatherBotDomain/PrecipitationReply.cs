using System.Text;

namespace WeatherBotDomain
{
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
                var readableTimeSegments = item.TimeSegments.Select(segment => segment.GetStringRepresentation());
                builder.Append(item.Description)
                    .Append(' ')
                    .Append(item.Emoji)
                    .Append(' ')
                    .Append(string.Join(", ", readableTimeSegments))
                    .Append('\n');
            }

            return builder.ToString();
        }
    }
}