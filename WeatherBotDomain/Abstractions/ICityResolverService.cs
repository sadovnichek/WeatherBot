using WeatherBotDomain.Entities;

namespace WeatherBotDomain.Abstractions
{
    public interface ICityResolverService
    {
        Result<Coordinates> TryResolve(string cityName);
    }
}