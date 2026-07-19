using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain.Abstractions
{
    public interface IWeatherApiClient
    {
        Task<string> TrySendRequestAsync(WeatherRequest request);
    }
}
