using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotInfrastructure.Database
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        public long ChatId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int LastQueryTime { get; set; }
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
        public DbSet<User> Users => Set<User>();

        public DbSet<City> Cities => Set<City>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeatherBotStorage", "app.db");
            optionsBuilder.UseSqlite(string.Format("Data Source={0}", path));
        }
    }
}