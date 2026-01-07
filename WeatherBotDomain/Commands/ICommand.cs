namespace WeatherBotDomain.Commands
{
    public interface ICommand
    {
        string Description { get; }

        Task<string> Execute(string[] args);
    }
}