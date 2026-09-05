using BotInfrastructure;
using System.Text;
using WeatherBotDomain.Commands;

namespace WeatherBotDomain.Replies
{
    public class WeeklyReply : Reply
    {
        public IReadOnlyList<DailyWeatherData> AggregatedData { get; }

        public WeeklyReply(List<DailyWeatherData> aggregatedData)
        {
            AggregatedData = aggregatedData;
        }

        public override string BuildMessage()
        {
            var builder = new StringBuilder();

            foreach(var item in AggregatedData)
            {
                var emojies = string.Join("", item.Weather);
                builder.Append($"{item.Date.ToString("dd.MM")}\t{emojies}\t{Format.Temperature(item.MedianTemperature)}\n");
            }

            return builder.ToString();
        }
    }
}