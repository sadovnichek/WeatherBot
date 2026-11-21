namespace BotInfrastructure
{
    public class UserInfo
    {
        public required long ChatId { get; init; }

        public required long LastMessageTimeUtc { get; init; }
    }
}