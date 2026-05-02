using BotInfrastructure;

namespace WeatherBotDomain.Commands
{
    public class StartCommand : ICommand
    {
        public string Description => "начать работу с ботом";

        public async Task<Reply?> Execute(string[] args)
        {
            return new PlainReply("""
                Добро пожаловать в WeatherBot! 
                Бот умеет давать прогноз погоды, температуру воздуха и информацию об осадках на сегодня/завтра.
                Достаточно ввести команды /today или /tomorrow.
                Больше команд доступны в /help
                """);
        }
    }
}