using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class HelpCommand : ICommand
    {
        private readonly CommandHandler commandHandler;

        public string Description => string.Empty;

        public HelpCommand(CommandHandler handler)
        {
            commandHandler = handler;
        }

        public Task<string> Execute(string[] args)
        {
            return Task.Run(() => string.Join("\n", commandHandler.GetCommands().Select(x => $"{x.Key} {x.Value.Description}")));
        }
    }
}
