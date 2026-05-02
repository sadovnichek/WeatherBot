namespace WeatherBotDomain
{
    public interface IWeatherApiController
    {
        Task<WeatherApiResponse?> TrySendRequest();
    }
}