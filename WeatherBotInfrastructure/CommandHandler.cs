using System.Collections.Frozen;
using System.Threading.Channels;

namespace BotInfrastructure
{
    public class CommandHandler
    {
        private Dictionary<string, ICommand> botCommands;

        public CommandHandler(FrozenDictionary<string, ICommand> commands,
            HelpCommand help)
        {
            botCommands = commands.ToDictionary(kv => kv.Key, kv => kv.Value);
            botCommands.Add("/help", help);
        }

        public bool IsCommandExists(string command)
        {
            return botCommands.ContainsKey(command);
        }

        /// <exception cref="ArgumentException"></exception>
        public async Task HandleCommand(string command, string[] args)
        {
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
    }
}