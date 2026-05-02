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

        public HourlyForecastReply(List<HourlyForecastData> data)
        {
            _data = data;
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
                  .Append($"{item.Temperature}°C\n");
            }

            return sb.ToString();
        }
    }
}