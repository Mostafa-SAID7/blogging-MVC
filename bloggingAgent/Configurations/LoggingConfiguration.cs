using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Configurations
{
    public static class LoggingConfiguration
    {
        public static WebApplicationBuilder AddLoggingConfiguration(
            this WebApplicationBuilder builder)
        {
            // Configure Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();

            // Configure in Development
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();
                builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            }

            return builder;
        }
    }
}
