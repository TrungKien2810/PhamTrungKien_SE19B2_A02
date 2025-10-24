using Microsoft.Extensions.Configuration;
using System.IO;

namespace PhamTrungKienDAL
{
    public static class AdminConfig
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

        public static string AdminEmail => 
            Configuration["AdminAccount:Email"] ?? "admin@FUMiniHotelSystem.com";

        public static string AdminPassword => 
            Configuration["AdminAccount:Password"] ?? "@@abc123@@";
    }
}

