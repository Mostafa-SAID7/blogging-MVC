using Microsoft.Extensions.DependencyInjection;
using BloggingAgent.Utilities;

namespace BloggingAgent.Configurations
{
    public static class HealthCheckConfiguration
    {
        public static IServiceCollection AddHealthCheckConfiguration(
            this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("DatabaseHealthCheck");

            return services;
        }
    }
}
