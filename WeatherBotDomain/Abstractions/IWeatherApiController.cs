using WeatherBotDomain.Entities;

namespace WeatherBotDomain.Abstractions
{
    public interface IWeatherApiController
    {
        WeatherInfo? TryProcessApiResponse(string json);
    }
}