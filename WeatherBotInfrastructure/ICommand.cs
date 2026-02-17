namespace BotInfrastructure
{
    public interface ICommand
    {
        string Description { get; }

        IAsyncEnumerable<IReply> Execute(string[] args);
    }
}