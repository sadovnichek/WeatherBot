namespace BotInfrastructure
{
    public interface ICommand
    {
        string Description { get; }

        string Name { get; }

        Task<Reply?> Execute(UserInfo userInfo, string[] args);
    }
}