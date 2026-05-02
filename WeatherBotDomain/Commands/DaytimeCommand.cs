using BotInfrastructure;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Commands
{
    public class DaytimeCommand : ICommand
    {
        private readonly IWeatherApiController _controller;

        public string Description => "Время заката и рассвета сегодня";

        public DaytimeCommand(IWeatherApiController controller)
        {
            _controller = controller;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            var dto = await _controller.TrySendRequest();

            if (dto is null)
                return null;

            var segment = new TimeSegment(dto.Sunrise, dto.Sunset);

            return new DaytimeReply(segment);
        }
    }
}