namespace BotInfrastructure
{
    public class CommandHandler
    {
        private Dictionary<string, ICommand> botCommands;

        public CommandHandler(IMessageBus<Message> bus)
        {
            botCommands = new Dictionary<string, ICommand>();
        }

        public CommandHandler(IMessageBus<Message> bus, Dictionary<string, ICommand> commands)
        {
            botCommands = commands;
        }

        public bool RegisterCommand(string command, ICommand executor)
        {
            return botCommands.TryAdd(command, executor);
        }

        public bool IsCommandExists(string command)
        {
            return botCommands.ContainsKey(command);
        }

        public async Task<string> HandleCommand(long chatId, string command, string[] args)
        {
            if (botCommands.TryGetValue(command, out var instance))
            {
                await instance.Execute(args);
            }

            throw new ArgumentException($"Unknown command {command}");
        }

        public IEnumerable<string> GetCommands()
        {
            return botCommands.Keys;
        }

        public string GetCommandDescription(string command)
        {
            if (botCommands.TryGetValue(command, out var instance))
            {
                return instance.Description;
            }

            throw new ArgumentException($"Unknown command {command}");
        }
    }
}