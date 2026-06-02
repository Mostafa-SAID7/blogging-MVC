using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.Email;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using AutoMapper;
using BloggingAgent.Services.Mapping.Profiles;
using BloggingAgent.Services.SocialMedia;
using BloggingAgent.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace BloggingAgent.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBloggingAgentServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();

            // Register Services
            services.AddScoped<ILlmConnector, LlmConnector>();
            services.AddScoped<IMemoryService, MemoryService>();
            services.AddScoped<ISeoService, SeoService>();
            services.AddScoped<IContentFormatter, ContentFormatter>();
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<ISocialMediaService, SocialMediaService>();
            services.AddScoped<IMemoryAnalyzer, MemoryAnalyzer>();
            services.AddScoped<ISeoAnalyzer, SeoAnalyzer>();
            // HttpContext accessor for services that need request info (e.g., syndication)
            services.AddHttpContextAccessor();
            services.AddScoped<ISyndicationService, SyndicationService>();

            // AutoMapper profiles
            services.AddAutoMapper(typeof(BlogProfile).Assembly);

            // Register LLM Providers
            services.AddScoped<ILlmProvider, OpenAIProvider>();
            services.AddScoped<ILlmProvider, OllamaProvider>();

            // Register Agents
            services.AddScoped<IBloggingAgent, BloggingAgent.Agents.BloggingAgent>();

            // Note: TextAnalyzer, SlugGenerator, WordCounter are static classes and don't need DI registration
            // Note: Middleware (ErrorHandlingMiddleware, RequestLoggingMiddleware) are registered via UseMiddleware<>, not DI

            return services;
        }
    }
}