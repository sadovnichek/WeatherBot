using System.Collections.Frozen;
using System.Threading.Channels;

namespace BotInfrastructure
{
    public class HelpCommand : ICommand
    {
        private FrozenDictionary<string, ICommand> _commands;
        private ChannelWriter<string> _bus;

        public string Description => throw new NotImplementedException();

        public HelpCommand(FrozenDictionary<string, ICommand> commands,
            ChannelWriter<string> bus)
        {
            _commands = commands;
            _bus = bus;
        }

        public async Task Execute(string[] args)
        {
            await _bus.WriteAsync(GetHelp());
        }

        private string GetHelp()
        {
            return string.Join("\n", _commands.Select(kv => $"{kv.Key} {kv.Value.Description}"));
        }
    }
}