using System.Text;

namespace WeatherBotDomain.Reply
{
    public class HourlyForecastReply : IReply
    {
        private readonly string[] _args;
        private readonly WeatherCore _weatherDomain;
        private readonly OpenMeteoResponse _response;

        public HourlyForecastReply(string[] args,
            WeatherCore weatherDomain, OpenMeteoResponse response)
        {
            _args = args;
            _weatherDomain = weatherDomain;
            _response = response;
        }

        public string BuildMessage()
        {
            var sb = new StringBuilder();

            var start = _args.Length == 0
                ? 0
                : int.Parse(_args[0]);
            var end = _args.Length == 0
                ? 24
                : int.Parse(_args[1]) + 1;

            for (var i = start; i < end; i++)
            {
                sb.Append($"{i.ToString().PadLeft(2, '0')}:00 ");
                sb.Append(_weatherDomain.GetEmoji(_response.WeatherData.WeatherCodes[i]));
                sb.Append(' ');
                sb.Append($"{_response.WeatherData.TemperaturePoints[i]}°C\n");
            }

            return sb.ToString();
        }
    }
}
