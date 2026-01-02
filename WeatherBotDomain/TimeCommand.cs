using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    internal class TimeCommand : ICommand
    {
        public string Execute(string[] args)
        {
            return DateTime.Now.ToString();
        }
    }
}
