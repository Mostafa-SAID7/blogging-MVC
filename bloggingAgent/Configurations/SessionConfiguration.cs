using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BloggingAgent.Configurations
{
    public static class SessionConfiguration
    {
        public static IServiceCollection AddSessionConfiguration(
            this IServiceCollection services)
        {
            // Configure Session (relaxed for Replit proxy)
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            return services;
        }
    }
}
