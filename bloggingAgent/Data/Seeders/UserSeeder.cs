using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Data.Seeders
{
    public class UserSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(UserManager<ApplicationUser> userManager, ILogger<UserSeeder> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting user seeding...");

            await SeedAdminUserAsync();
            await SeedDemoAuthorAsync();
            await SeedDemoReaderAsync();

            _logger.LogInformation("User seeding completed");
        }

        private async Task SeedAdminUserAsync()
        {
            const string email = "admin@bloggingagent.com";
            
            if (await _userManager.FindByEmailAsync(email) != null)
                return;

            var adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "System",
                LastName = "Administrator",
                Bio = "The system administrator for BloggingAgent",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Administrator");
                _logger.LogInformation("Created admin user: {Email}", email);
            }
            else
            {
                _logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private async Task SeedDemoAuthorAsync()
        {
            const string email = "author@bloggingagent.com";
            
            if (await _userManager.FindByEmailAsync(email) != null)
                return;

            var demoAuthor = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "Demo",
                LastName = "Author",
                Bio = "A passionate content creator using AI to generate amazing blog posts",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(demoAuthor, "Author123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoAuthor, "Author");
                _logger.LogInformation("Created demo author: {Email}", email);
            }
            else
            {
                _logger.LogError("Failed to create demo author: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private async Task SeedDemoReaderAsync()
        {
            const string email = "reader@bloggingagent.com";
            
            if (await _userManager.FindByEmailAsync(email) != null)
                return;

            var demoReader = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "Demo",
                LastName = "Reader",
                Bio = "A blog reader interested in AI and technology",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(demoReader, "Reader123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoReader, "Reader");
                _logger.LogInformation("Created demo reader: {Email}", email);
            }
            else
            {
                _logger.LogError("Failed to create demo reader: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
