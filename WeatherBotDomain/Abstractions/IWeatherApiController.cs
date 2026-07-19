using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain.Abstractions
{
    public interface IWeatherApiController
    {
        WeatherInfo? TryProcessApiResponse(string json);
    }
}