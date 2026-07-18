using BotInfrastructure;
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

        public async Task<Reply?> Execute(string[] args)
        {
            var request = new OpenMeteoWeatherRequest() { Latitude = 56.82, Longitude = 60.55 };
            var json = await _client.TrySendRequestAsync(request);
            var dto = _controller.TryProcessApiResponse(json);

            if (dto is null)
                return null;

            var segment = new TimeSegment(dto.Sunrise, dto.Sunset);

            return new DaytimeReply(segment);
        }
    }
}