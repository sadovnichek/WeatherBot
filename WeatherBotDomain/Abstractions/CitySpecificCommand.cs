using BotInfrastructure;
using WeatherBotDomain.Entities;

namespace WeatherBotDomain.Abstractions
{
    public abstract class CitySpecificCommand : ICommand
    {
        private ICityResolverService _cityResolverService;

        public abstract string Description { get; }
        public abstract string Name { get; }

        public CitySpecificCommand(ICityResolverService cityResolverService)
        {
            _cityResolverService = cityResolverService;
        }

        public abstract Task<Reply?> Execute(string[] args);

        protected bool TryResolveCoordinates(string[] args, out string? message, out Coordinates coordinates)
        {
            if (args.Length != 1)
            {
                // check if default city set
                message = "Укажите город, например: /today Москва";
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