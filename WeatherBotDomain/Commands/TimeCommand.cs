namespace WeatherBotDomain.Commands
{
    public class TimeCommand : ICommand
    {
        public string Description => "описание команды время";

        public async Task<string> Execute(string[] args)
        {
            return await Task.Run(() => DateTime.Now.ToString());
        }
    }
}