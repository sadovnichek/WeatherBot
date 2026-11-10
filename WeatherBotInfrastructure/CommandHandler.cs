namespace BotInfrastructure
{
    public interface ICommandHandler
    {
        bool TryGetCommand(string name, out ICommand command);

        IEnumerable<ICommand> GetAllCommands();
    }

    public class CommandHandler : ICommandHandler
    {
        private Dictionary<string, ICommand> _commands;

        public CommandHandler(IEnumerable<ICommand> commands)
        {
            _commands = commands.ToDictionary(c => c.Name, c => c);
        }

        public CommandHandler(IEnumerable<ICommand> commands,
            IInfrastructureCommand helpCommand) : this(commands)
        {
            _commands.Add(helpCommand.Name, helpCommand);
        }

        public bool TryGetCommand(string name, out ICommand command)
        {
            if (!_commands.TryGetValue(name, out var result))
            {
                command = null;
                return false;
            }

            command = result;
            return true;
        }

        public IEnumerable<ICommand> GetAllCommands()
        {
            return _commands.Values;
        }
    }
}