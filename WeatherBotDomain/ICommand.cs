using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    internal interface ICommand
    {
        string Execute(string[] args);
    }
}
