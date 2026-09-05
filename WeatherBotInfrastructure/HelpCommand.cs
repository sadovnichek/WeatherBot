namespace BotInfrastructure
{
    public class HelpCommand : ICommand
    {
        private Dictionary<string, ICommand> _commands;

        public string Description => "вывести список команд";

        public HelpCommand(Dictionary<string, ICommand> commands)
        {
            _commands = commands;
        }

        public async Task<Reply?> Execute(string[] args)
        {
            return new PlainReply(GetHelp());
        }

        private string GetHelp()
        {
            return string.Join("\n",
                _commands.Select(kv => $"{kv.Key} {kv.Value.Description}"));
        }
    }
}