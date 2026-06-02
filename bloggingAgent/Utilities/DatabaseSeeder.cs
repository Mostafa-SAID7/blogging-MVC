using BloggingAgent.Data;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloggingAgent.Utilities
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            // Create roles
            var roles = new[] { "Admin", "Editor", "Author", "Reader" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Created role: {Role}", role);
                }
            }

            // Create default admin user
            var adminEmail = "admin@bloggingagent.com";
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    Bio = "Default system administrator",
                    AvatarUrl = "",
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Created default admin user");
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Seed default agent settings
            if (!await _context.AgentSettings.AnyAsync())
            {
                _context.AgentSettings.Add(new AgentSettings
                {
                    DefaultAuthor = "AI Assistant",
                    MaxPostLength = 5000,
                    DefaultTags = new List<string> { "blog", "ai-generated" },
                    AutoPublish = false,
                    Theme = "default",
                    CustomSettings = new Dictionary<string, object>
                    {
                        ["EnableAutoSeo"] = true,
                        ["DefaultTone"] = "professional",
                        ["ContentQualityThreshold"] = 80
                    }
                });

                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded default agent settings");
            }

            // Seed sample blog posts for development
            if (_context.BlogPosts.Count() == 0 && _logger.IsEnabled(LogLevel.Debug))
            {
                await SeedSamplePostsAsync();
            }
        }

        private async Task SeedSamplePostsAsync()
        {
            var samplePosts = new[]
            {
                new BlogPost
                {
                    Title = "Welcome to BloggingAgent",
                    Slug = "welcome-to-bloggingagent",
                    Content = @"# Welcome to BloggingAgent

This is your first AI-powered blog post! BloggingAgent uses advanced artificial intelligence to help you create engaging, SEO-optimized content.

## Features

- **AI Content Generation**: Create high-quality blog posts with AI assistance
- **SEO Optimization**: Built-in SEO analysis and optimization tools
- **Content Analytics**: Track performance and engagement metrics
- **Multi-format Support**: Generate content for blogs, social media, and email

## Getting Started

1. Navigate to the **Generate** page to create new content
2. Use the **Analytics** dashboard to track performance
3. Customize settings in the **Settings** panel

Happy blogging!",
                    Excerpt = "Welcome to BloggingAgent - your AI-powered content creation platform",
                    Author = "AI Assistant",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7),
                    Status = PostStatus.Published,
                    Tags = new List<string> { "welcome", "ai", "blogging" }
                },
                new BlogPost
                {
                    Title = "Understanding AI Content Generation",
                    Slug = "understanding-ai-content-generation",
                    Content = @"# Understanding AI Content Generation

Artificial Intelligence has revolutionized content creation, making it possible to generate high-quality written material quickly and efficiently.

## How AI Content Generation Works

AI content generation uses machine learning models trained on vast amounts of text data to understand patterns, context, and language structure.

### Key Benefits

- **Speed**: Generate content in seconds rather than hours
- **Consistency**: Maintain consistent tone and style
- **Scalability**: Produce large volumes of content
- **Cost-Effective**: Reduce content creation costs

## Best Practices

1. **Review and Edit**: Always review AI-generated content
2. **Add Personal Touch**: Include unique insights and experiences
3. **Fact-Check**: Verify all facts and statistics
4. **Optimize for SEO**: Use appropriate keywords and structure

## Future of AI Content

The future of AI content generation looks promising, with continuous improvements in quality, creativity, and understanding of nuanced topics.",
                    Excerpt = "Learn how AI content generation works and its benefits for modern content creators",
                    Author = "AI Assistant",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Status = PostStatus.Published,
                    Tags = new List<string> { "ai", "content", "technology" }
                }
            };

            foreach (var post in samplePosts)
            {
                _context.BlogPosts.Add(post);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} sample blog posts", samplePosts.Length);
        }
    }
}