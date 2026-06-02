using System.Diagnostics;
using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IBloggingAgent _bloggingAgent;
        private readonly ICacheService _cacheService;

        public HomeController(
            ILogger<HomeController> logger,
            IRepository<BlogPost> blogPostRepository,
            IRepository<ApplicationUser> userRepository,
            IBloggingAgent bloggingAgent,
            ICacheService cacheService)
        {
            _logger = logger;
            _blogPostRepository = blogPostRepository;
            _userRepository = userRepository;
            _bloggingAgent = bloggingAgent;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                const string cacheKey = "home_page_data";
                var cachedModel = await _cacheService.GetAsync<HomeViewModel>(cacheKey);

                if (cachedModel != null)
                {
                    return View(cachedModel);
                }

                // Get featured posts (latest published posts)
                var featuredPosts = await GetFeaturedPostsAsync(6);

                // Get platform statistics
                var stats = await GetPlatformStatsAsync();

                // Get recent activity
                var recentActivity = await GetRecentActivityAsync(5);

                // Get popular categories
                var popularCategories = await GetPopularCategoriesAsync(6);

                var model = new HomeViewModel
                {
                    FeaturedPosts = featuredPosts,
                    PlatformStats = stats,
                    RecentActivity = recentActivity,
                    PopularCategories = popularCategories,
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false
                };

                // Cache for 10 minutes
                await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(10));

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page content.");
                Response.StatusCode = 500;

                var errorModel = new ErrorViewModel
                {
                    StatusCode = 500,
                    Title = "Application Error",
                    Message = "We were unable to load the home page due to an internal error.",
                    DetailedMessage = "Please try again later or contact support if the issue persists.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                return View("Error", errorModel);
            }
        }

        [HttpGet]
        public IActionResult About()
        {
            var model = new AboutViewModel
            {
                PlatformName = "BloggingAgent",
                Version = "1.0.0",
                Description = "An AI-powered blogging platform that helps content creators generate, optimize, and publish high-quality blog posts with advanced SEO capabilities.",
                Features = new List<string>
                {
                    "AI-Powered Content Generation",
                    "Advanced SEO Optimization",
                    "Social Media Integration",
                    "Real-time Analytics",
                    "Comment Management",
                    "User Community",
                    "Multi-platform Publishing",
                    "Content Scheduling"
                },
                Technologies = new List<string>
                {
                    "ASP.NET Core 9.0",
                    "Entity Framework Core",
                    "SQL Server",
                    "OpenAI GPT",
                    "React.js",
                    "Bootstrap 5",
                    "Docker"
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Features()
        {
            var model = new FeaturesViewModel
            {
                FeatureCategories = new List<FeatureCategory>
                {
                    new FeatureCategory
                    {
                        Name = "AI Content Creation",
                        Icon = "fas fa-robot",
                        Description = "Generate high-quality blog posts using advanced AI technology",
                        Features = new List<string>
                        {
                            "Topic research and content ideation",
                            "Automated content generation",
                            "SEO-optimized writing",
                            "Multiple tone and style options",
                            "Content expansion and editing"
                        }
                    },
                    new FeatureCategory
                    {
                        Name = "SEO Optimization",
                        Icon = "fas fa-search",
                        Description = "Built-in SEO tools to improve search engine rankings",
                        Features = new List<string>
                        {
                            "Keyword research and analysis",
                            "Meta description generation",
                            "Content optimization suggestions",
                            "SEO performance tracking",
                            "Competitor analysis"
                        }
                    },
                    new FeatureCategory
                    {
                        Name = "Social Media Integration",
                        Icon = "fas fa-share-alt",
                        Description = "Seamlessly share content across social media platforms",
                        Features = new List<string>
                        {
                            "Auto-posting to multiple platforms",
                            "Social media scheduling",
                            "Engagement tracking",
                            "Platform-specific optimization",
                            "Performance analytics"
                        }
                    },
                    new FeatureCategory
                    {
                        Name = "Analytics & Insights",
                        Icon = "fas fa-chart-line",
                        Description = "Comprehensive analytics to track content performance",
                        Features = new List<string>
                        {
                            "Real-time traffic monitoring",
                            "Content engagement metrics",
                            "SEO performance tracking",
                            "Audience demographics",
                            "Conversion tracking"
                        }
                    },
                    new FeatureCategory
                    {
                        Name = "Community Management",
                        Icon = "fas fa-users",
                        Description = "Build and engage with your content community",
                        Features = new List<string>
                        {
                            "Comment management system",
                            "User interaction tracking",
                            "Community notifications",
                            "User profile management",
                            "Social networking features"
                        }
                    },
                    new FeatureCategory
                    {
                        Name = "Content Management",
                        Icon = "fas fa-edit",
                        Description = "Professional content management and publishing tools",
                        Features = new List<string>
                        {
                            "Visual content editor",
                            "Content scheduling",
                            "Version control",
                            "Multi-author support",
                            "Content workflow management"
                        }
                    }
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Pricing()
        {
            var model = new PricingViewModel
            {
                Plans = new List<PricingPlan>
                {
                    new PricingPlan
                    {
                        Name = "Free",
                        Price = 0,
                        Period = "forever",
                        Description = "Perfect for getting started with AI-powered blogging",
                        Features = new List<string>
                        {
                            "5 blog posts per month",
                            "Basic AI content generation",
                            "SEO analysis",
                            "Community access",
                            "Email support"
                        },
                        ButtonText = "Get Started",
                        ButtonClass = "btn-outline-primary",
                        IsPopular = false
                    },
                    new PricingPlan
                    {
                        Name = "Pro",
                        Price = 29,
                        Period = "month",
                        Description = "Advanced features for growing bloggers and content creators",
                        Features = new List<string>
                        {
                            "Unlimited blog posts",
                            "Advanced AI features",
                            "Social media integration",
                            "Advanced analytics",
                            "Priority support",
                            "Custom branding",
                            "API access"
                        },
                        ButtonText = "Start Pro Trial",
                        ButtonClass = "btn-primary",
                        IsPopular = true
                    },
                    new PricingPlan
                    {
                        Name = "Enterprise",
                        Price = 99,
                        Period = "month",
                        Description = "Complete solution for teams and large organizations",
                        Features = new List<string>
                        {
                            "Everything in Pro",
                            "Multi-user accounts",
                            "Advanced permissions",
                            "White-label solution",
                            "Dedicated support",
                            "Custom integrations",
                            "SLA guarantee"
                        },
                        ButtonText = "Contact Sales",
                        ButtonClass = "btn-outline-dark",
                        IsPopular = false
                    }
                },
                FAQs = new List<FAQ>
                {
                    new FAQ
                    {
                        Question = "Can I change plans at any time?",
                        Answer = "Yes, you can upgrade or downgrade your plan at any time. Changes take effect immediately."
                    },
                    new FAQ
                    {
                        Question = "Is there a free trial?",
                        Answer = "Yes, we offer a 14-day free trial for all paid plans with full access to all features."
                    },
                    new FAQ
                    {
                        Question = "What payment methods do you accept?",
                        Answer = "We accept all major credit cards, PayPal, and bank transfers for annual plans."
                    },
                    new FAQ
                    {
                        Question = "Can I cancel my subscription?",
                        Answer = "Yes, you can cancel your subscription at any time. You'll continue to have access until the end of your billing period."
                    }
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            var model = new ContactViewModel
            {
                ContactInfo = new ContactInfo
                {
                    Email = "support@bloggingagent.com",
                    Phone = "+1 (555) 123-4567",
                    Address = "123 AI Street, Tech City, TC 12345",
                    BusinessHours = "Monday - Friday: 9:00 AM - 6:00 PM EST"
                },
                SocialLinks = new List<SocialLink>
                {
                    new SocialLink { Platform = "Twitter", Url = "https://twitter.com/bloggingagent", Icon = "fab fa-twitter" },
                    new SocialLink { Platform = "LinkedIn", Url = "https://linkedin.com/company/bloggingagent", Icon = "fab fa-linkedin" },
                    new SocialLink { Platform = "GitHub", Url = "https://github.com/bloggingagent", Icon = "fab fa-github" }
                }
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            Response.StatusCode = statusCode ?? 500;

            var errorModel = new ErrorViewModel
            {
                StatusCode = statusCode,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            if (statusCode.HasValue)
            {
                switch (statusCode.Value)
                {
                    case 404:
                        errorModel.Title = "Page Not Found";
                        errorModel.Message = "We couldn't find the page you were looking for.";
                        errorModel.DetailedMessage = "The link may be broken or the page may have been removed.";
                        break;
                    case 403:
                        errorModel.Title = "Access Denied";
                        errorModel.Message = "You don't have permission to view this page.";
                        errorModel.DetailedMessage = "If you believe this is a mistake, please contact support.";
                        break;
                    default:
                        errorModel.Title = "Unexpected Error";
                        errorModel.Message = "An error occurred while processing your request.";
                        errorModel.DetailedMessage = "Please try again later or return to the home page.";
                        break;
                }
            }
            else
            {
                var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
                if (exceptionFeature != null)
                {
                    _logger.LogError(exceptionFeature.Error, "Unhandled exception routed to error page.");
                    errorModel.Title = "Application Error";
                    errorModel.Message = "Something went wrong while processing your request.";
                    errorModel.DetailedMessage = "Our team has been notified and we are working to resolve the issue.";
                }
            }

            return View(errorModel);
        }

        private async Task<List<BlogPostDto>> GetFeaturedPostsAsync(int count)
        {
            var posts = await _blogPostRepository.GetAllAsync();
            return posts.Where(p => p.IsPublished)
                       .OrderByDescending(p => p.CreatedAt)
                       .Take(count)
                       .Select(p => new BlogPostDto
                       {
                           Id = p.Id,
                           Title = p.Title,
                           Slug = p.Slug,
                           Excerpt = p.Excerpt,
                           Author = p.Author,
                           CreatedAt = p.CreatedAt,
                           Tags = p.Tags
                       })
                       .ToList();
        }

        private async Task<PlatformStats> GetPlatformStatsAsync()
        {
            var posts = await _blogPostRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            return new PlatformStats
            {
                TotalPosts = posts.Count(p => p.IsPublished),
                TotalUsers = users.Count(),
                TotalComments = posts.Sum(p => p.Analytics?.Comments ?? 0),
                TotalViews = posts.Sum(p => p.Analytics?.Views ?? 0)
            };
        }

        private async Task<List<RecentActivity>> GetRecentActivityAsync(int count)
        {
            var posts = await _blogPostRepository.GetAllAsync();
            return posts.Where(p => p.IsPublished)
                       .OrderByDescending(p => p.CreatedAt)
                       .Take(count)
                       .Select(p => new RecentActivity
                       {
                           Type = "post",
                           Title = $"New post: {p.Title}",
                           Description = p.Excerpt?.Substring(0, Math.Min(100, p.Excerpt.Length)) + "...",
                           Url = $"/blog/{p.Slug}",
                           Timestamp = p.CreatedAt
                       })
                       .ToList();
        }

        private async Task<List<CategoryInfo>> GetPopularCategoriesAsync(int count)
        {
            var posts = await _blogPostRepository.GetAllAsync();
            var categoryStats = posts.Where(p => p.IsPublished)
                                   .SelectMany(p => p.Tags)
                                   .GroupBy(tag => tag)
                                   .Select(g => new CategoryInfo
                                   {
                                       Name = g.Key,
                                       PostCount = g.Count(),
                                       Slug = g.Key.ToLower().Replace(" ", "-")
                                   })
                                   .OrderByDescending(c => c.PostCount)
                                   .Take(count)
                                   .ToList();

            return categoryStats;
        }
    }

}
