using BotInfrastructure;
using System.Threading.Channels;

namespace WeatherBotDomain.Commands
{
    public class StartCommand : ICommand
    {
        private ChannelWriter<string> messageBus;

        public string Description => "начать работу с ботом";

        public StartCommand(ChannelWriter<string> bus)
        {
            messageBus = bus;
        }

        public async Task Execute(string[] args)
        {
            await messageBus.WriteAsync("""
                Добро пожаловать в WeatherBot! 
                Бот умеет давать прогноз погоды, температуру воздуха и информацию об осадках на сегодня/завтра.
                Достаточно ввести команды /today или /tomorrow.
                Больше команд доступны в /help
                """);
        }
    }
}