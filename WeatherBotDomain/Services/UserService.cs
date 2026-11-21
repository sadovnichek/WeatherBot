using BotInfrastructure.Database;
using Microsoft.Extensions.Logging;
using WeatherBotDomain.Abstractions;

namespace WeatherBotDomain.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<IUserService> _logger;

        public UserService(AppDbContext dbContext,
            ILogger<IUserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public User GetUserByChatId(long chatId)
        {
            var dbRecord = _dbContext.Users.FirstOrDefault(u => u.ChatId == chatId);

            if (dbRecord is null)
                throw new ArgumentException($"Unable to find user with Chat id = {chatId}");

            return dbRecord;
        }

        public User CreateUser(long chatId, long currentTime)
        {
            try
            {
                var newUser = new User() { ChatId = chatId, LastQueryTime = currentTime };
                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create user");
                throw;
            }
        }

        public User UpdateUserDefaultCityId(long chatId, long defaultCityId)
        {
            var user = GetUserByChatId(chatId);

            user.DefaultCityId = defaultCityId;
            _dbContext.SaveChanges();
            return user;
        }

        public User UpdateUserLastQueryTime(long chatId, long lastQueryTime)
        {
            var user = GetUserByChatId(chatId);

            user.LastQueryTime = lastQueryTime;
            _dbContext.SaveChanges();
            return user;
        }

        public bool UserExists(long chatId)
        {
            return _dbContext.Users.Any(u => u.ChatId == chatId);
        }

        public User CreateOrUpdate(long chatId, long lastQueryTime)
        {
            if (!UserExists(chatId))
            {
                return CreateUser(chatId, lastQueryTime);
            }

            return UpdateUserLastQueryTime(chatId, lastQueryTime);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dbContext.Users;
        }
    }
}