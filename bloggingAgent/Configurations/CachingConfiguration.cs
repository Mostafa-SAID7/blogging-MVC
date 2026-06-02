using Microsoft.Extensions.DependencyInjection;

namespace BloggingAgent.Configurations
{
    public static class CachingConfiguration
    {
        public static IServiceCollection AddCachingConfiguration(
            this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();

            return services;
        }
    }
}
