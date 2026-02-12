using System.Collections.Frozen;
using System.Threading.Channels;

namespace BotInfrastructure
{
    public class HelpCommand : ICommand
    {
        private Dictionary<string, ICommand> _commands;
        private ChannelWriter<string> _bus;

        public string Description => "вывести список команд";

        public HelpCommand(Dictionary<string, ICommand> commands,
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
            return string.Join("\n",
                _commands.Select(kv => $"{kv.Key} {kv.Value.Description}"));
        }
    }
}