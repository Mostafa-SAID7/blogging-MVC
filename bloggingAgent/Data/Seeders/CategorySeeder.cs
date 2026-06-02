using BloggingAgent.Models.Domain;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Data.Seeders
{
    public class CategorySeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategorySeeder> _logger;

        public CategorySeeder(ApplicationDbContext context, ILogger<CategorySeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting category seeding...");

            try
            {
                var categories = GetCategories();

                foreach (var category in categories)
                {
                    try
                    {
                        if (!_context.Categories.Any(c => c.Slug == category.Slug))
                        {
                            _context.Categories.Add(category);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Added category: {CategoryName}", category.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding category: {CategoryName}", category.Name);
                        // Continue with next category instead of failing entirely
                    }
                }

                _logger.LogInformation("Category seeding completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during category seeding orchestration");
                // Don't rethrow - allow app to continue
            }
        }

        private List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    Name = "Artificial Intelligence",
                    Slug = "artificial-intelligence",
                    Description = "Posts about AI, machine learning, and intelligent systems",
                    Icon = "fa-brain",
                    Color = "#FF6B6B",
                    IsActive = true
                },
                new Category
                {
                    Name = "Technology",
                    Slug = "technology",
                    Description = "Latest technology trends and innovations",
                    Icon = "fa-microchip",
                    Color = "#4ECDC4",
                    IsActive = true
                },
                new Category
                {
                    Name = "Programming",
                    Slug = "programming",
                    Description = "Programming tutorials, tips, and best practices",
                    Icon = "fa-code",
                    Color = "#45B7D1",
                    IsActive = true
                },
                new Category
                {
                    Name = "Web Development",
                    Slug = "web-development",
                    Description = "Web development frameworks, tools, and techniques",
                    Icon = "fa-globe",
                    Color = "#96CEB4",
                    IsActive = true
                },
                new Category
                {
                    Name = "Data Science",
                    Slug = "data-science",
                    Description = "Data analysis, visualization, and machine learning",
                    Icon = "fa-chart-line",
                    Color = "#FFEAA7",
                    IsActive = true
                },
                new Category
                {
                    Name = "Digital Marketing",
                    Slug = "digital-marketing",
                    Description = "SEO, social media, and online marketing strategies",
                    Icon = "fa-bullhorn",
                    Color = "#DDA15E",
                    IsActive = true
                },
                new Category
                {
                    Name = "Business",
                    Slug = "business",
                    Description = "Business strategies, entrepreneurship, and management",
                    Icon = "fa-briefcase",
                    Color = "#BC6C25",
                    IsActive = true
                },
                new Category
                {
                    Name = "Lifestyle",
                    Slug = "lifestyle",
                    Description = "Personal development, productivity, and lifestyle tips",
                    Icon = "fa-heart",
                    Color = "#EF476F",
                    IsActive = true
                }
            };
        }
    }
}
