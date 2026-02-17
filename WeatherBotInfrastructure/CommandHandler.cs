namespace BotInfrastructure
{
    public class CommandHandler
    {
        private Dictionary<string, ICommand> botCommands;

        public CommandHandler(Dictionary<string, ICommand> commands)
        {
            botCommands = commands;
        }

        public bool IsCommandExists(string command)
        {
            return botCommands.ContainsKey(command);
        }

        /// <exception cref="ArgumentException"></exception>
        public async IAsyncEnumerable<IReply> HandleCommand(string command, string[] args)
        {
            if(!botCommands.TryGetValue(command, out var instance)) 
                throw new ArgumentException($"Unknown command {command}");

            await foreach (var reply in instance.Execute(args))
            {
                yield return reply;
            }
        }
    }
}