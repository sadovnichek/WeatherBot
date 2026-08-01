using BotInfrastructure;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : ICommand
    {
        private readonly IWeatherApiController _controller;
        private readonly IWeatherApiClient _client;

        public string Description => "Время заката и рассвета сегодня";

        public DaytimeCommand(IWeatherApiController controller, IWeatherApiClient client)
        {
            _controller = controller;
            _client = client;
        }

        public async Task<Reply> Execute(string[] args)
        {
            var request = new OpenMeteoWeatherRequest() 
            { 
                Latitude = double.Parse(args[0]), 
                Longitude = double.Parse(args[1]),
                ForecastDays = 1
            };

            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return PlainReply.OnError();

            var segment = new TimeSegment(dto.Sunrise, dto.Sunset);

            return new DaytimeReply(segment);
        }

        public bool AreArgumentsValid(string[] args) => true;
    }
}