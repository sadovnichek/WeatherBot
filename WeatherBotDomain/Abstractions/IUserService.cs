using BotInfrastructure.Database;

namespace WeatherBotDomain.Abstractions
{
    public interface IUserService
    {
        User GetUserByChatId(long chatId);

        User UpdateUserLastQueryTime(long chatId, long lastQueryTime);

        User UpdateUserDefaultCityId(long chatId, long defaultCityId);

        User CreateUser(long chatId, long currentTime);

        bool UserExists(long chatId);

        User CreateOrUpdate(long chatId, long lastQueryTime);

        IEnumerable<User> GetAllUsers();
    }
}