using BotInfrastructure.Database;
using WeatherBotDomain.Entities;

namespace WeatherBotDomain.Abstractions
{
    public interface ICityResolverService
    {
        Result<Coordinates> TryResolve(string cityName);

        Result<long> GetCityIdByName(string cityName);

        Result<City> GetCityById(long cityId);
    }
}