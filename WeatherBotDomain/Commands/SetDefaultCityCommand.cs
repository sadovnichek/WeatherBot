using BotInfrastructure;
using WeatherBotDomain.Abstractions;

namespace WeatherBotDomain.Commands
{
    public class SetDefaultCityCommand : ICommand
    {
        private readonly ICityResolverService _cityResolverService;
        private readonly IUserService _userService;

        public string Description => "задать город по умолчанию";

        public string Name => "/setDefaultCity";

        public SetDefaultCityCommand(ICityResolverService cityResolverService,
            IUserService userService)
        {
            _cityResolverService = cityResolverService;
            _userService = userService;
        }

        public async Task<Reply?> Execute(UserInfo userInfo, string[] args)
        {
            _userService.CreateOrUpdate(userInfo.ChatId, userInfo.LastMessageTimeUtc);

            var cityName = args[0];
            var result = _cityResolverService.GetCityIdByName(cityName);
            if (!result.Success)
                return new PlainReply("Не удалось определить город");

            var chatId = userInfo.ChatId;
            try
            {
                _userService.UpdateUserDefaultCityId(chatId, result.Value);
                return new PlainReply($"Город {cityName} установлен");
            }
            catch (Exception)
            {
                return new PlainReply("Команда завершилась с ошибкой");
            }
        }
    }
}