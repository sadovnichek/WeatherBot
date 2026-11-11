using BotInfrastructure.Database;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Entities;

namespace WeatherBotDomain.Services
{
    public class CityResolverService : ICityResolverService
    {
        private readonly AppDbContext _dbContext;

        public CityResolverService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieve city coordinates by it own name
        /// </summary>
        /// <returns>false if city does not exist, otherwise true and city coordinates</returns>
        public Result<Coordinates> TryResolve(string cityName)
        {
            var dbRecord = _dbContext.Cities.FirstOrDefault(x => x.Name == cityName);

            if (dbRecord is null)
            {
                return Result<Coordinates>.Failure();
            }

            var coordinates = new Coordinates(dbRecord.Latitude, dbRecord.Longitude);
            return new Result<Coordinates>(coordinates);
        }
    }
}