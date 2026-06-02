using Microsoft.AspNetCore.Builder;

namespace BloggingAgent.Configurations
{
    public static class RoutingConfiguration
    {
        public static WebApplication MapApplicationRoutes(
            this WebApplication app)
        {
            // Map Routes
            app.MapControllerRoute(
                name: "blog",
                pattern: "blog/{action=Index}/{id?}",
                defaults: new { controller = "Blog" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Blog}/{action=Index}/{id?}");

            // Health Check Endpoint
            app.MapHealthChecks("/health");

            // API Routes
            app.MapControllers();

            return app;
        }
    }
}
