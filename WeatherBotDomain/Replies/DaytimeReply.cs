using BotInfrastructure;

namespace WeatherBotDomain.Replies
{
    public class DaytimeReply(TimeSegment TimeSegment) : Reply
    {
        private readonly string sunriseEmoji = "☀️";
        private readonly string sunsetEmoji = "🌙";

        public override string BuildMessage()
        {
            return $"Рассвет: {sunriseEmoji} {TimeSegment.Start}\nЗакат: {sunsetEmoji} {TimeSegment.End}";
        }
    }
}