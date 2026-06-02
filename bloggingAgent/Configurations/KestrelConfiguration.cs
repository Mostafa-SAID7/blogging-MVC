using Microsoft.AspNetCore.Builder;
using System.Net;

namespace BloggingAgent.Configurations
{
    public static class KestrelConfiguration
    {
        public static WebApplicationBuilder AddKestrelConfiguration(
            this WebApplicationBuilder builder)
        {
            // Configure Kestrel to listen on 0.0.0.0:5000
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5000);
            });

            return builder;
        }
    }
}
