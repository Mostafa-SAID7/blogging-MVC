using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BloggingAgent.Data;

namespace BloggingAgent.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                        sqlOptions.CommandTimeout(300); // 5 minutes timeout
                    }
                ));

            return services;
        }
    }
}
