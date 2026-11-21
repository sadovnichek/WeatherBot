using BotInfrastructure.Database;
using Microsoft.Extensions.Logging;
using WeatherBotDomain.Entities;

namespace WeatherBotDomain.Abstractions
{
    public interface ICoordinateResolver
    {
        bool TryResolveCoordinates(User user, string[] args, out string? message, out Coordinates coordinates);
    }

    public class CoordinateResolver : ICoordinateResolver
    {
        private ICityResolverService _cityResolverService;
        private ILogger<CoordinateResolver> _logger;

        public CoordinateResolver(ICityResolverService cityResolverService,
            ILogger<CoordinateResolver> logger)
        {
            _cityResolverService = cityResolverService;
            _logger = logger;
        }

        public bool TryResolveCoordinates(User user, 
            string[] args, out string? message, out Coordinates coordinates)
        {
            if (args.Length != 1)
            {
                if (user.DefaultCityId.HasValue)
                {
                    var cityRequestResult = _cityResolverService.GetCityById(user.DefaultCityId.Value);
                    if (cityRequestResult.Success)
                    {
                        message = default;
                        coordinates = new Coordinates(cityRequestResult.Value.Latitude, cityRequestResult.Value.Longitude);
                        return true;
                    }
                    else
                    {
                        _logger.LogError("User {userId} has default city Id {cityId} that incorrect",
                            user.Id,
                            cityRequestResult.Value.Id);
                    }
                }
                message = $"Укажите город, например: /today Москва";
                coordinates = default;
                return false;
            }

            var resolveResult = _cityResolverService.TryResolve(args[0]);
            if (!resolveResult.Success)
            {
                message = "Не удалось определить город";
                coordinates = default;
                return false;
            }

            message = default;
            coordinates = resolveResult.Value;
            return true;
        }
    }
}