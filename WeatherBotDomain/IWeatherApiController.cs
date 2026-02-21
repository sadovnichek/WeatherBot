using Newtonsoft.Json;
using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain
{
    public interface IWeatherApiController
    {
        Task<WeatherDTO?> TrySendRequest();
    }
}