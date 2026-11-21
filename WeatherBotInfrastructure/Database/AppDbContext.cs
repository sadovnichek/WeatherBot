using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BotInfrastructure.Database
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        public long ChatId { get; set; }

        public long? DefaultCityId { get; set; }

        public long LastQueryTime { get; set; }

        public override string ToString()
        {
            return $"\nId={Id}\nChatId={ChatId}\nDefaultCityId={DefaultCityId}\nLastQueryTime={LastQueryTime}\n";
        }
    }

    public class City
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Utc { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();

        public DbSet<City> Cities => Set<City>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeatherBotStorage", "wb.db");
            optionsBuilder.UseSqlite($"Data Source = {path}");
        }
    }
}