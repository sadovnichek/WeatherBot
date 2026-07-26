using BotInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                builder.Append($"{item.Date} {emojies} {Format.Temperature(item.MedianTemperature)}\n");
            }

            return builder.ToString();
        }
    }
}