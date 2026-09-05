namespace WeatherBotDomain.Abstractions
{
    public interface IWeatherApiClient
    {
        Task<string> TrySendRequestAsync(WeatherRequest request);
    }
}