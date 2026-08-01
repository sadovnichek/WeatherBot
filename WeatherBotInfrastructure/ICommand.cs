namespace BotInfrastructure
{
    public interface ICommand
    {
        string Description { get; }

        Task<Reply?> Execute(string[] args);

        bool AreArgumentsValid(string[] args);
    }
}