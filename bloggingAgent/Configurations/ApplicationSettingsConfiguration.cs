using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BloggingAgent.Settings;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Configurations
{
    public static class ApplicationSettingsConfiguration
    {
        public static IServiceCollection AddApplicationSettingsConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure LLM Settings
            services.Configure<LlmSettings>(configuration.GetSection("LlmSettings"));

            // Configure OpenAI Settings
            services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));

            // Configure SEO Settings
            services.Configure<SeoSettings>(configuration.GetSection("SeoSettings"));

            // Configure Cache Settings
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

            // Configure Content Settings
            services.Configure<ContentSettings>(configuration.GetSection("ContentSettings"));

            // Configure Agent Settings
            services.Configure<AgentSettings>(configuration.GetSection("AgentSettings"));

            // Configure Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Configure Social Media Settings
            services.Configure<SocialMediaSettings>(configuration.GetSection("SocialMediaSettings"));

            return services;
        }
    }
}
