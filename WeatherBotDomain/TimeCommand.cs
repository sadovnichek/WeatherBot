using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    internal class TimeCommand : ICommand
    {
        public async Task<string> Execute(string[] args)
        {
            return await Task.Run(() => DateTime.Now.ToString());
        }
    }
}