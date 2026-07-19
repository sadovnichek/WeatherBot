using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain
{
    public interface IWeatherApiController
    {
        WeatherInfo? TryProcessApiResponse(string json);
    }
}