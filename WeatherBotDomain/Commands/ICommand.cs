using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain.Commands
{
    public interface ICommand
    {
        string Description { get; }

        Task<string> Execute(string[] args);
    }
}
