using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using BloggingAgent.Middleware;

namespace BloggingAgent.Configurations
{
    public static class MiddlewareConfiguration
    {
        public static WebApplication UseApplicationMiddleware(
            this WebApplication app)
        {
            // Custom Middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseResponseCaching();

            return app;
        }
    }
}
