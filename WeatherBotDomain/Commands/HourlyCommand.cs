using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;
using WeatherBotDomain.Services;

namespace WeatherBotDomain.Commands
{
    public class HourlyCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;
        private readonly WeatherCore weatherDomain;
        private readonly CoordinateArgumentResolver _coordinateArgumentResolver;

        public string Description => "Погода и температура на каждый час сегодня";

        public HourlyCommand(IWeatherApiController controller, 
            IWeatherApiClient client,
            WeatherCore domain,
            CoordinateArgumentResolver coordinateArgumentResolver)
        {
            _controller = controller;
            weatherDomain = domain;
            _client = client;
            _coordinateArgumentResolver = coordinateArgumentResolver;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            if (!_coordinateArgumentResolver.TryGetCoordinates(args, out var coordinates, out var message))
                return new PlainReply(message);

            var request = new OpenMeteoWeatherRequest() 
            {
                Coordinates = coordinates,
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
    }
}