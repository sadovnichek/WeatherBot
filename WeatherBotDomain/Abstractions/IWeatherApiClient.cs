using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain
{
    public interface IWeatherApiClient
    {
        Task<string> TrySendRequestAsync(WeatherRequest request);
    }
}
