using Microsoft.Extensions.Logging;
using BloggingAgent.Data.Seeders;

namespace BloggingAgent.Data
{
    /// <summary>
    /// Main orchestrator for database seeding.
    /// Coordinates the seeding of all database entities through specialized seeder classes.
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly RoleSeeder _roleSeeder;
        private readonly UserSeeder _userSeeder;
        private readonly CategorySeeder _categorySeeder;
        private readonly BlogPostSeeder _blogPostSeeder;
        private readonly AgentSettingsSeeder _agentSettingsSeeder;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            RoleSeeder roleSeeder,
            UserSeeder userSeeder,
            CategorySeeder categorySeeder,
            BlogPostSeeder blogPostSeeder,
            AgentSettingsSeeder agentSettingsSeeder,
            ILogger<DatabaseSeeder> logger)
        {
            _roleSeeder = roleSeeder;
            _userSeeder = userSeeder;
            _categorySeeder = categorySeeder;
            _blogPostSeeder = blogPostSeeder;
            _agentSettingsSeeder = agentSettingsSeeder;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                await _roleSeeder.SeedAsync();
                await _userSeeder.SeedAsync();
                await _categorySeeder.SeedAsync();
                await _blogPostSeeder.SeedAsync();
                await _agentSettingsSeeder.SeedAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database seeding");
                throw;
            }
        }
    }
}