using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PhamTrungKienDAL
{
    public static class DatabaseConfig
    {
        private static IConfiguration? _configuration;
        
        private static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    
                    _configuration = builder.Build();
                }
                return _configuration;
            }
        }

        public static string ConnectionString => 
            Configuration.GetConnectionString("DefaultConnection") ?? 
            "Server=.;Database=FUMiniHotelManagement;User Id=sa;Password=kingqn132;TrustServerCertificate=True;Encrypt=False;";

        public static void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}

