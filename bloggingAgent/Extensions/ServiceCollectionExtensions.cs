using BloggingAgent.Agents;
using BloggingAgent.Configuration;
using BloggingAgent.Data;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Middleware;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloggingAgent.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBloggingAgentServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();

            // Services
            services.AddScoped<ILlmConnector, LlmConnector>();
            services.AddScoped<IMemoryService, MemoryService>();
            services.AddScoped<ISeoService, SeoService>();
            services.AddScoped<IContentFormatter, ContentFormatter>();
            services.AddScoped<ICacheService, MemoryCacheService>();

            // Agents
            services.AddScoped<IBloggingAgent, BloggingAgent.Agents.BloggingAgent>();

            // Configuration
            services.Configure<LlmSettings>(configuration.GetSection("LlmSettings"));
            services.Configure<OpenAISettings>(configuration.GetSection("OpenAISettings"));
            services.Configure<SeoSettings>(configuration.GetSection("SeoSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            services.Configure<Models.Domain.AgentSettings>(configuration.GetSection("AgentSettings"));

            // LLM Providers
            services.AddHttpClient<OpenAIProvider>();
            services.AddHttpClient<OllamaProvider>();
            services.AddScoped<ILlmProvider, OpenAIProvider>();
            services.AddScoped<ILlmProvider, OllamaProvider>();

            // Memory Cache
            services.AddMemoryCache();

            return services;
        }

        public static IApplicationBuilder UseBloggingAgentMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();

            return app;
        }
    }
}