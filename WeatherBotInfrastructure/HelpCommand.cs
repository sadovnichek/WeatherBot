namespace BotInfrastructure
{
    public interface IInfrastructureCommand : ICommand
    {

    }

    public class HelpCommand : IInfrastructureCommand
    {
        private readonly IEnumerable<ICommand> _commands;

        public string Description => "вывести список команд";

        public string Name => "/help";

        public HelpCommand(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public async Task<Reply?> Execute(UserInfo userInfo, string[] args)
        {
            return new PlainReply(GetHelp());
        }

        private string GetHelp()
        {
            return string.Join("\n",
                _commands.Select(c => $"{c.Name} {c.Description}"));
        }
    }
}