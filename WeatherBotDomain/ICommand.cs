using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    internal interface ICommand
    {
        Task<string> Execute(string[] args);
    }
}
