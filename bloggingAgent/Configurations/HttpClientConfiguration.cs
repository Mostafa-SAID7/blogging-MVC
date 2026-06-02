using Microsoft.Extensions.DependencyInjection;
using System;

namespace BloggingAgent.Configurations
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddHttpClientConfiguration(
            this IServiceCollection services)
        {
            services.AddHttpClient("OpenAI", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Add("User-Agent", "BloggingAgent/1.0");
            });

            services.AddHttpClient("Ollama", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                client.DefaultRequestHeaders.Add("User-Agent", "BloggingAgent/1.0");
            });

            return services;
        }
    }
}
