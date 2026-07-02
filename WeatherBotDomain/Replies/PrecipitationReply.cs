using BotInfrastructure;
using System.Text;

namespace WeatherBotDomain.Replies
{
    public class PrecipitationReply : Reply
    {
        public IEnumerable<WeatherSegment> WeatherSegments { get; }

        public PrecipitationReply(IEnumerable<WeatherSegment> weatherSegments)
        {
            WeatherSegments = weatherSegments;
        }

        public override string BuildMessage()
        {
            if (!WeatherSegments.Any())
                return "Осадки не ожидаются";

            var builder = new StringBuilder();
            var joinedSegments = TimeSegment.Join(WeatherSegments.SelectMany(x => x.TimeSegments).ToArray())
                .Select(s => s.GetStringRepresentation());
            builder.Append($"Ожидаются осадки: { string.Join(", ", joinedSegments) }\n");

            foreach (var item in WeatherSegments)
            {
                var readableTimeSegments = item.TimeSegments.Select(segment => segment.GetStringRepresentation());
                builder.Append($"*{item.Description}*")
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