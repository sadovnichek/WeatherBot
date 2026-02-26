namespace WeatherBotDomain
{
    public interface IWeatherApiController
    {
        Task<WeatherReply?> TrySendRequest();
    }
}