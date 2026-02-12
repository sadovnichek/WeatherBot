using System.Collections.Frozen;
using System.Threading.Channels;

namespace BotInfrastructure
{
    public class HelpCommand : ICommand
    {
        private FrozenDictionary<string, ICommand> _commands;
        private ChannelWriter<string> _bus;
        private Func<string> _getHelp;

        public string Description => "вывести список команд";

        public HelpCommand(FrozenDictionary<string, ICommand> commands,
            ChannelWriter<string> bus)
        {
            _commands = commands;
            _bus = bus;
            _getHelp = GetHelp;
        }

        public async Task Execute(string[] args)
        {
            await _bus.WriteAsync(_getHelp());
        }

        private string GetHelp()
        {
            return string.Join("\n",
                _commands.Select(kv => $"{kv.Key} {kv.Value.Description}")
                .Append($"/help {Description}"));
        }
    }
}