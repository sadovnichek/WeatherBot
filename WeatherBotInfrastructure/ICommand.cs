namespace BotInfrastructure
{
    public interface ICommand
    {
        string Description { get; }

        Task Execute(string[] args);
    }
}