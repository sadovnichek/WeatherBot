using BotInfrastructure;
using System.Text.RegularExpressions;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Commands
{
    public class HourlyCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly WeatherCore weatherDomain;

        public string Description => "Погода и температура на каждый час сегодня";

        public HourlyCommand(IWeatherApiController controller, IWeatherApiClient client,
            WeatherCore domain)
        {
            _controller = controller;
            weatherDomain = domain;
            _client = client;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            if (!AreArgumentsValid(args))
            {
                return new PlainReply(
                    """
                    Неверные аргументы.
                    Нужно указать два числа - начало и конец временного промежутка от 0 до 23 часов
                    """);
            }

            var request = new OpenMeteoWeatherRequest() 
            { 
                Latitude = 56.82, 
                Longitude = 60.55,
                ForecastDays = 1
            };
            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError(); ;

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

            return reply;
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