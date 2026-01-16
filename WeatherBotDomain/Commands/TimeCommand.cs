using BotInfrastructure;

namespace WeatherBotDomain.Commands
{
    public class TimeCommand : ICommand
    {
        public string Description => "описание команды время";

        public async Task Execute(string[] args)
        {
            await Task.Run(() => DateTime.Now.ToString());
        }
    }
}