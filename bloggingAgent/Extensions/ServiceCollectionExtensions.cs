using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
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
            services.AddScoped<MemoryAnalyzer>();
            services.AddScoped<SeoAnalyzer>();

            // Register LLM Providers
            services.AddScoped<ILlmProvider, OpenAIProvider>();
            services.AddScoped<ILlmProvider, OllamaProvider>();

            // Register Agents
            services.AddScoped<IBloggingAgent, BloggingAgent>();

            // Register Utilities
            services.AddScoped<TextAnalyzer>();
            services.AddScoped<SlugGenerator>();
            services.AddScoped<WordCounter>();

            // Register Middleware
            services.AddScoped<Middleware.ErrorHandlingMiddleware>();
            services.AddScoped<Middleware.RequestLoggingMiddleware>();

            return services;
        }
    }
}