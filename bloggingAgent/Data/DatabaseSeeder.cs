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

                try { await _roleSeeder.SeedAsync(); }
                catch (Exception ex) { _logger.LogError(ex, "Error seeding roles"); }

                try { await _userSeeder.SeedAsync(); }
                catch (Exception ex) { _logger.LogError(ex, "Error seeding users"); }

                try { await _categorySeeder.SeedAsync(); }
                catch (Exception ex) { _logger.LogError(ex, "Error seeding categories"); }

                try { await _blogPostSeeder.SeedAsync(); }
                catch (Exception ex) { _logger.LogError(ex, "Error seeding blog posts"); }

                try { await _agentSettingsSeeder.SeedAsync(); }
                catch (Exception ex) { _logger.LogError(ex, "Error seeding agent settings"); }

                _logger.LogInformation("Database seeding completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during database seeding orchestration");
                // Don't rethrow - allow partial seeding to succeed
            }
        }
    }
}