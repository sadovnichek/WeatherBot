using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class TimeCommand : ICommand
    {
        public string Description => "описание команды время";

        public async Task<string> Execute(string[] args)
        {
            return await Task.Run(() => DateTime.Now.ToString());
        }
    }
}