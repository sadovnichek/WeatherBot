using BotInfrastructure;
using WeatherBotDomain.Reply;

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

        public async IAsyncEnumerable<IReply> Execute(string[] args)
        {
            var dto = await _controller.TrySendRequest();

            if (dto is null)
                yield break;

            var segment = new TimeSegment(dto.Sunrise, dto.Sunset);

            yield return new DaytimeReply(segment);
        }
    }
}