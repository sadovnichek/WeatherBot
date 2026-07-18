using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain
{
    public interface IWeatherApiController
    {
        WeatherApiResponse? TryProcessApiResponse(string json);
    }

    public interface IWeatherApiClient
    {
        Task<string> TrySendRequestAsync(WeatherRequest request);
    }
}