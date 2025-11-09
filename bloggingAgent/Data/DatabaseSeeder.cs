using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Data
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
            try
            {
                _logger.LogInformation("Starting database seeding...");

                await SeedRolesAsync();
                await SeedUsersAsync();
                await SeedCategoriesAsync();
                await SeedSamplePostsAsync();
                await SeedAgentSettingsAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database seeding");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "Administrator", "Editor", "Author", "Reader" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Created role: {RoleName}", roleName);
                    }
                    else
                    {
                        _logger.LogError("Failed to create role {RoleName}: {Errors}",
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@bloggingagent.com",
                Email = "admin@bloggingagent.com",
                FirstName = "System",
                LastName = "Administrator",
                Bio = "The system administrator for BloggingAgent",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Administrator");
                    _logger.LogInformation("Created admin user: {Email}", adminUser.Email);
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Create demo author
            var demoAuthor = new ApplicationUser
            {
                UserName = "author@bloggingagent.com",
                Email = "author@bloggingagent.com",
                FirstName = "Demo",
                LastName = "Author",
                Bio = "A passionate content creator using AI to generate amazing blog posts",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            if (await _userManager.FindByEmailAsync(demoAuthor.Email) == null)
            {
                var result = await _userManager.CreateAsync(demoAuthor, "Author123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(demoAuthor, "Author");
                    _logger.LogInformation("Created demo author: {Email}", demoAuthor.Email);
                }
            }

            // Create demo reader
            var demoReader = new ApplicationUser
            {
                UserName = "reader@bloggingagent.com",
                Email = "reader@bloggingagent.com",
                FirstName = "Demo",
                LastName = "Reader",
                Bio = "A blog reader interested in AI and technology",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            if (await _userManager.FindByEmailAsync(demoReader.Email) == null)
            {
                var result = await _userManager.CreateAsync(demoReader, "Reader123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(demoReader, "Reader");
                    _logger.LogInformation("Created demo reader: {Email}", demoReader.Email);
                }
            }
        }

        private async Task SeedCategoriesAsync()
        {
            var categories = new[]
            {
                new Category { Name = "Artificial Intelligence", Slug = "artificial-intelligence", Description = "Posts about AI, machine learning, and intelligent systems", IsActive = true },
                new Category { Name = "Technology", Slug = "technology", Description = "Latest technology trends and innovations", IsActive = true },
                new Category { Name = "Programming", Slug = "programming", Description = "Programming tutorials, tips, and best practices", IsActive = true },
                new Category { Name = "Web Development", Slug = "web-development", Description = "Web development frameworks, tools, and techniques", IsActive = true },
                new Category { Name = "Data Science", Slug = "data-science", Description = "Data analysis, visualization, and machine learning", IsActive = true },
                new Category { Name = "Digital Marketing", Slug = "digital-marketing", Description = "SEO, social media, and online marketing strategies", IsActive = true },
                new Category { Name = "Business", Slug = "business", Description = "Business strategies, entrepreneurship, and management", IsActive = true },
                new Category { Name = "Lifestyle", Slug = "lifestyle", Description = "Personal development, productivity, and lifestyle tips", IsActive = true }
            };

            foreach (var category in categories)
            {
                if (!_context.Categories.Any(c => c.Slug == category.Slug))
                {
                    _context.Categories.Add(category);
                    _logger.LogInformation("Added category: {CategoryName}", category.Name);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedSamplePostsAsync()
        {
            if (_context.BlogPosts.Any())
            {
                _logger.LogInformation("Sample posts already exist, skipping...");
                return;
            }

            var author = await _userManager.FindByEmailAsync("author@bloggingagent.com");
            if (author == null) return;

            var aiCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == "artificial-intelligence");
            var techCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == "technology");

            var samplePosts = new[]
            {
                new BlogPost
                {
                    Title = "Getting Started with AI-Powered Blogging",
                    Slug = "getting-started-ai-blogging",
                    Content = @"# Getting Started with AI-Powered Blogging

Artificial Intelligence has revolutionized the way we create content. With AI-powered blogging platforms like BloggingAgent, you can generate high-quality blog posts in minutes instead of hours.

## Why AI Blogging?

AI blogging offers several advantages:

- **Speed**: Generate complete articles in minutes
- **Consistency**: Maintain consistent quality and tone
- **SEO Optimization**: Built-in SEO analysis and optimization
- **Scalability**: Create content at scale without fatigue

## How BloggingAgent Works

BloggingAgent uses advanced language models to understand your requirements and generate engaging content. Simply provide a topic, keywords, and target audience, and let the AI do the rest.

## Best Practices

1. **Define Clear Objectives**: Know what you want to achieve
2. **Provide Context**: Give the AI enough information to work with
3. **Review and Edit**: Always review AI-generated content
4. **Optimize for SEO**: Use the built-in SEO tools

## Future of AI Blogging

The future of blogging is AI-assisted, not AI-replaced. AI will handle the heavy lifting while human creativity and expertise guide the direction and strategy.

Start your AI blogging journey today with BloggingAgent!",
                    Excerpt = "Discover how AI-powered blogging can transform your content creation process and help you reach your audience more effectively.",
                    AuthorId = author.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7),
                    IsPublished = true,
                    Tags = new List<string> { "AI", "Blogging", "Content Creation", "SEO" },
                    SeoMetadata = new SeoMetadata
                    {
                        Title = "Getting Started with AI-Powered Blogging | BloggingAgent",
                        Description = "Learn how AI-powered blogging platforms can help you create better content faster. Discover the benefits and best practices for AI-assisted blogging.",
                        Keywords = "AI blogging, content creation, artificial intelligence, blogging platform",
                        CanonicalUrl = "/blog/getting-started-ai-blogging"
                    },
                    Analytics = new ContentAnalytics
                    {
                        Views = 1250,
                        UniqueViews = 980,
                        Shares = 45,
                        Comments = 12,
                        AverageReadTime = 4.5,
                        BounceRate = 0.25,
                        LastUpdated = DateTime.UtcNow
                    }
                },

                new BlogPost
                {
                    Title = "The Rise of Machine Learning in Modern Applications",
                    Slug = "rise-machine-learning-modern-applications",
                    Content = @"# The Rise of Machine Learning in Modern Applications

Machine Learning (ML) has become an integral part of modern software applications. From recommendation systems to autonomous vehicles, ML algorithms are powering innovations across industries.

## What is Machine Learning?

Machine Learning is a subset of artificial intelligence that enables systems to learn and improve from experience without being explicitly programmed. It uses algorithms to identify patterns in data and make predictions.

## Applications in Today's World

### E-commerce
- **Personalized Recommendations**: Amazon, Netflix use ML to suggest products/content
- **Dynamic Pricing**: Airlines and hotels adjust prices based on demand patterns
- **Fraud Detection**: Banks use ML to identify suspicious transactions

### Healthcare
- **Disease Diagnosis**: ML algorithms assist in medical imaging analysis
- **Drug Discovery**: AI helps identify potential drug candidates faster
- **Patient Monitoring**: Wearables use ML for health monitoring

### Transportation
- **Autonomous Vehicles**: Self-driving cars rely on ML for navigation
- **Traffic Optimization**: Smart cities use ML for traffic flow management
- **Ride Sharing**: Uber, Lyft use ML for pricing and routing

## Future Trends

The future of ML includes:

1. **Edge Computing**: Running ML models on devices
2. **Federated Learning**: Privacy-preserving distributed learning
3. **Explainable AI**: Making ML decisions more transparent
4. **AutoML**: Automated machine learning model development

## Getting Started with ML

If you're interested in machine learning:

1. **Learn Python**: Most ML libraries are Python-based
2. **Study Mathematics**: Linear algebra, calculus, statistics
3. **Practice with Datasets**: Use platforms like Kaggle
4. **Start Small**: Begin with simple projects and build up

Machine Learning is transforming how we build software and solve problems. The possibilities are endless!",
                    Excerpt = "Explore how machine learning is revolutionizing modern applications across industries and what the future holds for this transformative technology.",
                    AuthorId = author.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    IsPublished = true,
                    Tags = new List<string> { "Machine Learning", "AI", "Technology", "Innovation" },
                    SeoMetadata = new SeoMetadata
                    {
                        Title = "The Rise of Machine Learning in Modern Applications",
                        Description = "Discover how machine learning is transforming industries from healthcare to transportation. Learn about current applications and future trends.",
                        Keywords = "machine learning, artificial intelligence, applications, technology trends",
                        CanonicalUrl = "/blog/rise-machine-learning-modern-applications"
                    },
                    Analytics = new ContentAnalytics
                    {
                        Views = 890,
                        UniqueViews = 720,
                        Shares = 28,
                        Comments = 8,
                        AverageReadTime = 6.2,
                        BounceRate = 0.30,
                        LastUpdated = DateTime.UtcNow
                    }
                },

                new BlogPost
                {
                    Title = "SEO Best Practices for 2024",
                    Slug = "seo-best-practices-2024",
                    Content = @"# SEO Best Practices for 2024

Search Engine Optimization continues to evolve as search engines become more sophisticated. Staying updated with the latest SEO practices is crucial for online visibility.

## Core SEO Fundamentals

### Technical SEO
- **Mobile-First Design**: Ensure your site works perfectly on mobile devices
- **Page Speed**: Optimize loading times (target under 3 seconds)
- **HTTPS**: Secure your site with SSL certificates
- **XML Sitemap**: Help search engines discover your content

### Content SEO
- **Quality Content**: Create valuable, comprehensive content
- **Keyword Research**: Use tools to find relevant search terms
- **User Intent**: Understand what users are looking for
- **Content Structure**: Use proper headings and formatting

### Off-Page SEO
- **Backlinks**: Build quality inbound links
- **Social Signals**: Engage on social media platforms
- **Local SEO**: Optimize for local search if applicable
- **Brand Mentions**: Monitor and encourage brand mentions

## 2024 SEO Trends

### AI and Machine Learning
- **AI-Powered Search**: Google uses AI for better search results
- **Content Generation**: AI tools for content creation and optimization
- **Voice Search**: Optimize for conversational queries
- **Featured Snippets**: Aim for position zero

### User Experience Focus
- **Core Web Vitals**: Google's UX metrics become ranking factors
- **Page Experience**: Overall user experience matters
- **Mobile UX**: Mobile-friendliness is critical
- **Accessibility**: WCAG compliance helps SEO

### Evolving Search Features
- **Rich Results**: Structured data for enhanced search results
- **Local Pack**: Optimize for local search features
- **Video SEO**: YouTube and video content optimization
- **App Indexing**: Mobile app content in search results

## SEO Tools and Resources

### Free Tools
- **Google Search Console**: Monitor your site's presence in Google
- **Google Analytics**: Track user behavior and conversions
- **Google PageSpeed Insights**: Test page loading performance
- **Google Keyword Planner**: Research keywords

### Paid Tools
- **Ahrefs**: Comprehensive SEO analysis
- **SEMrush**: Competitive analysis and keyword research
- **Moz Pro**: SEO management and reporting
- **Screaming Frog**: Technical SEO auditing

## Measuring SEO Success

### Key Metrics
- **Organic Traffic**: Visitors from search engines
- **Keyword Rankings**: Positions for target keywords
- **Conversion Rate**: Goal completions from organic traffic
- **Domain Authority**: Overall site strength

### Tools for Tracking
- **Google Analytics**: Comprehensive analytics
- **Search Console**: Search performance data
- **Third-party Tools**: SEMrush, Ahrefs tracking

## Common SEO Mistakes to Avoid

1. **Keyword Stuffing**: Don't over-optimize with keywords
2. **Ignoring Mobile**: Mobile users exceed desktop
3. **Poor Site Speed**: Slow sites hurt user experience
4. **Thin Content**: Create comprehensive, valuable content
5. **Ignoring Analytics**: Track and measure your efforts

## Future of SEO

SEO will continue to evolve with:
- **AI Integration**: More AI-powered search and content tools
- **Voice Search**: Conversational search optimization
- **Visual Search**: Image and video search optimization
- **Privacy Changes**: Adapting to cookie-less tracking

Stay updated with SEO best practices and continuously optimize your content for better search visibility!",
                    Excerpt = "Master the latest SEO strategies for 2024. Learn technical SEO, content optimization, and emerging trends that will boost your search rankings.",
                    AuthorId = author.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    IsPublished = true,
                    Tags = new List<string> { "SEO", "Search Engine Optimization", "Digital Marketing", "2024 Trends" },
                    SeoMetadata = new SeoMetadata
                    {
                        Title = "SEO Best Practices for 2024 | Complete Guide",
                        Description = "Master SEO in 2024 with this comprehensive guide. Learn technical SEO, content optimization, and emerging trends for better search rankings.",
                        Keywords = "SEO, search engine optimization, SEO best practices, digital marketing, 2024 SEO trends",
                        CanonicalUrl = "/blog/seo-best-practices-2024"
                    },
                    Analytics = new ContentAnalytics
                    {
                        Views = 2100,
                        UniqueViews = 1650,
                        Shares = 89,
                        Comments = 23,
                        AverageReadTime = 8.7,
                        BounceRate = 0.20,
                        LastUpdated = DateTime.UtcNow
                    }
                }
            };

            foreach (var post in samplePosts)
            {
                _context.BlogPosts.Add(post);
                _logger.LogInformation("Added sample post: {Title}", post.Title);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedAgentSettingsAsync()
        {
            if (_context.AgentSettings.Any())
            {
                _logger.LogInformation("Agent settings already exist, skipping...");
                return;
            }

            var settings = new AgentSettings
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

            _context.AgentSettings.Add(settings);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added default agent settings");
        }
    }
}