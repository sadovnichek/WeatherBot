using BotInfrastructure.Database;

namespace WeatherBotDomain.Services
{
    public readonly struct Coordinates
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public interface ICityResolverService
    {
        bool TryResolve(string cityName, out Coordinates coordinates);
    }

    public class CityResolverService : ICityResolverService
    {
        private readonly AppDbContext _dbContext;

        public CityResolverService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieve city coordinates by name
        /// </summary>
        /// <returns>false if city does not exist, otherwise true and city coordinates</returns>
        public bool TryResolve(string cityName, out Coordinates coordinates)
        {
            var dbRecord = _dbContext.Cities.FirstOrDefault(x => x.Name == cityName);

            if (dbRecord is null)
            {
                coordinates = default;
                return false;
            }

            coordinates = new Coordinates(dbRecord.Latitude, dbRecord.Longitude);
            return true;
        }
    }
}