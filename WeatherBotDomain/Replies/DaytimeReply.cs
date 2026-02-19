using BotInfrastructure;

namespace WeatherBotDomain.Reply
{
    public record DaytimeReply(TimeSegment TimeSegment) : IReply
    {
        private readonly string sunriseEmoji = "☀️";
        private readonly string sunsetEmoji = "🌙";

        public string BuildMessage()
        {
            return $"Рассвет: {sunriseEmoji} {TimeSegment.Start}\nЗакат: {sunsetEmoji} {TimeSegment.End}";
        }
    }
}