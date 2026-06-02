using BloggingAgent.Settings;
using BloggingAgent.Data;
using BloggingAgent.Extensions;
using BloggingAgent.Configurations;
using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add Kestrel Configuration
builder.AddKestrelConfiguration();

// Add services to the container
builder.Services.AddControllersWithViews();

// Add Database Configuration
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Add Identity Configuration
builder.Services.AddIdentityConfiguration();

// Add Cookie Configuration
builder.Services.AddCookieConfiguration();

// Add Session Configuration
builder.Services.AddSessionConfiguration();

// Add Caching Configuration
builder.Services.AddCachingConfiguration();

// Add HTTP Client Configuration
builder.Services.AddHttpClientConfiguration();

// Add Application Settings Configuration
builder.Services.AddApplicationSettingsConfiguration(builder.Configuration);

// Add CORS Configuration
builder.Services.AddCorsConfiguration();

// Add Health Check Configuration
builder.Services.AddHealthCheckConfiguration();

// Add Logging Configuration
builder.AddLoggingConfiguration();

// Register Database Seeder
builder.Services.AddScoped<BloggingAgent.Utilities.DatabaseSeeder>();

// Register Services (moved to extension method for better organization)
builder.Services.AddBloggingAgentServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Do NOT use HTTPS redirection in Replit (proxy handles TLS)
// app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});

// Apply Middleware Configuration
app.UseApplicationMiddleware();

// Map Application Routes
app.MapApplicationRoutes();

// Database Initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var seeder = services.GetRequiredService<BloggingAgent.Utilities.DatabaseSeeder>();

        logger.LogInformation("Ensuring database exists and is up to date");
        await context.Database.EnsureCreatedAsync();

        logger.LogInformation("Seeding database with initial data");
        await seeder.SeedAsync();

        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

app.Run();
