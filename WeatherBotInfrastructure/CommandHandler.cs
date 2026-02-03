using System.Threading.Channels;

namespace BotInfrastructure
{
    public class CommandHandler
    {
        private Dictionary<string, ICommand> botCommands;
        private ChannelWriter<string> messageBus;

        public CommandHandler(ChannelWriter<string> bus)
        {
            botCommands = new Dictionary<string, ICommand>();
            messageBus = bus;
        }

        public CommandHandler(Dictionary<string, ICommand> commands, 
            ChannelWriter<string> bus)
        {
            botCommands = commands;
            messageBus = bus;
        }

        public bool RegisterCommand(string command, ICommand executor)
        {
            return botCommands.TryAdd(command, executor);
        }

        public bool IsCommandExists(string command)
        {
            if (command == "/help")
                return true;

            return botCommands.ContainsKey(command);
        }

        public async Task HandleCommand(string command, string[] args)
        {
            if (command == "/help")
            {
                await messageBus.WriteAsync(GetHelp());
                return;
            }

            if (botCommands.TryGetValue(command, out var instance))
            {
                await instance.Execute(args);
                return;
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

        public string GetHelp()
        {
            return string.Join("\n",
                GetCommands().Select(c => $"{c} {GetCommandDescription(c)}"));
        }
    }
}