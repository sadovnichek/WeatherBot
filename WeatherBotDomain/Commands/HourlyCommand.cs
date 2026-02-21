using BotInfrastructure;
using System.Text.RegularExpressions;
using WeatherBotDomain.Reply;

namespace WeatherBotDomain.Commands
{
    public class HourlyCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly WeatherCore weatherDomain;

        public string Description => "Погода и температура на каждый час сегодня";

        public HourlyCommand(IWeatherApiController controller,
            WeatherCore domain)
        {
            _controller = controller;
            weatherDomain = domain;
        }

        public async IAsyncEnumerable<IReply> Execute(string[] args)
        {
            if (!AreArgumentsValid(args))
            {
                yield return new PlainReply("""
                    Неверные аргументы.
                    Нужно указать два числа - начало и конец временного промежутка от 0 до 23 часов
                    """);
                yield break;
            }

            var dto = await _controller.TrySendRequest();

            if (dto is null)
                yield break;

            var startIndex = args.Length > 0 ? int.Parse(args[0]) : 0;
            var endIndex = args.Length > 0 ? int.Parse(args[1]) + 1 : 24;

            var reply = new HourlyForecastReply();
            var daytime = new TimeSegment(dto.Sunrise, dto.Sunset);
            for(var i = startIndex; i < endIndex; i++)
            {
                var time = new TimeOnly(i, 0);
                var isDay = daytime.IsTimeInSegment(time);
                var emoji = weatherDomain.GetEmoji(dto.WeatherCodes[i], !isDay);
                var temperature = dto.Temperatures[i];
                var data = new HourlyForecastData(time, emoji, temperature);
                reply.AppendData(data);
            }

            yield return reply;
        }

        private bool AreArgumentsValid(string[] args)
        {
            if (args.Length == 0)
                return true;

            if (args.Length == 2)
            {
                return Regex.IsMatch(args[0], @"\b([0-9]|1[0-9]|2[0-3])\b")
                    && Regex.IsMatch(args[1], @"\b([0-9]|1[0-9]|2[0-3])\b");
            }

            return false;
        }
    }
}