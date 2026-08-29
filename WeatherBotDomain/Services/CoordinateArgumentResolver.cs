using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain.Services
{
    public class CoordinateArgumentResolver
    {
        private readonly ICityResolverService _cityResolverService;

        public CoordinateArgumentResolver(ICityResolverService cityResolverService)
        {
            _cityResolverService = cityResolverService;
        }

        public bool TryGetCoordinates(string[] args,
            out Coordinates coordinates,
            out string message)
        {
            coordinates = default;
            message = string.Empty;
            if (args.Length == 1)
            {
                if (!_cityResolverService.TryResolve(args[0], out var c))
                {
                    message = "Город не найден";
                    return false;
                }

                coordinates = c;
                return true;
            }

            else if (args.Length == 2
                && double.TryParse(args[0], out var lat)
                && double.TryParse(args[1], out var lon))
            {
                coordinates = new Coordinates(lat, lon);
                return true;
            }

            message = "Неверные аргументы";
            return false;
        }
    }
}
