using BotInfrastructure;
using WeatherBotDomain.Abstractions;

namespace WeatherBotDomain.Commands
{
    public class StartCommand : ICommand
    {
        private readonly IUserService _userService;

        public string Description => "начать работу с ботом";

        public string Name => "/start";

        public StartCommand(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Reply?> Execute(UserInfo userInfo, string[] args)
        {
            _userService.CreateIfNotExists(userInfo.ChatId);
            _userService.UpdateUserLastQueryTime(userInfo.ChatId, userInfo.LastMessageTimeUtc);

            return Task.FromResult<Reply?>(new PlainReply("""
                Добро пожаловать в WeatherBot! 
                Бот умеет давать прогноз погоды, температуру воздуха и информацию об осадках на сегодня/завтра.
                Достаточно ввести команды /today или /tomorrow.
                Больше команд доступны в /help
                """));
        }
    }
}