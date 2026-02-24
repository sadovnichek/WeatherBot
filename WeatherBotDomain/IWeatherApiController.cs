namespace WeatherBotDomain
{
    public interface IWeatherApiController
    {
        Task<WeatherDTO?> TrySendRequest();
    }
}