using BotInfrastructure;
using System.Text;

namespace WeatherBotDomain.Replies
{
    public record HourlyForecastData(TimeOnly Time, string Emoji, double Temperature);

    public class HourlyForecastReply : Reply
    {
        private List<HourlyForecastData> _data;

        public HourlyForecastReply()
        {
            _data = new List<HourlyForecastData>();
        }

        public void AppendData(HourlyForecastData data)
        {
            _data.Add(data);
        }

        public override string BuildMessage()
        {
            var sb = new StringBuilder();

            foreach(var item in _data)
            {
                sb.Append(item.Time.ToShortTimeString())
                  .Append(' ')
                  .Append(item.Emoji)
                  .Append(' ')
                  .Append($"{Format.Temperature(item.Temperature)}\n");
            }

            return sb.ToString();
        }
    }
}