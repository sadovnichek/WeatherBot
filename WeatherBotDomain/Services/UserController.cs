using BotInfrastructure.Database;

namespace WeatherBotDomain.Services
{
    public interface IUserController
    {
        bool Create(int chatId);

        User GetUser(int chatId);
    }

    public class UserController /*: IUserController*/
    {
        private readonly AppDbContext _dbContext;
        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public bool Create(int chatId)
        //{
        //    var entity = _dbContext.Users.Add(new User { Id = chatId });
        //}
    }
}