using BloggingAgent.Models.Domain;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Data.Seeders
{
    public class AgentSettingsSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AgentSettingsSeeder> _logger;

        public AgentSettingsSeeder(ApplicationDbContext context, ILogger<AgentSettingsSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting agent settings seeding...");

            if (_context.AgentSettings.Any())
            {
                _logger.LogInformation("Agent settings already exist, skipping...");
                return;
            }

            var settings = GetDefaultAgentSettings();
            _context.AgentSettings.Add(settings);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added default agent settings");
            _logger.LogInformation("Agent settings seeding completed");
        }

        private AgentSettings GetDefaultAgentSettings()
        {
            return new AgentSettings
            {
                DefaultAuthor = "AI Assistant",
                MaxPostLength = 5000,
                DefaultTags = new List<string> { "blog", "ai-generated", "content" },
                AutoPublish = false,
                Theme = "default",
                CustomSettings = new Dictionary<string, object>
                {
                    ["EnableComments"] = true,
                    ["EnableSocialSharing"] = true,
                    ["EnableAnalytics"] = true,
                    ["MaxPostsPerDay"] = 10
                }
            };
        }
    }
}
