using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public interface IConnection
    {
        Task<string> SendRequest(string request, string uri);
    }
}
