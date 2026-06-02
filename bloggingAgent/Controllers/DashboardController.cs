using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<ContentAnalytics> _analyticsRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IBloggingAgent _bloggingAgent;
        private readonly ICacheService _cacheService;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IRepository<BlogPost> blogPostRepository,
            IRepository<Comment> commentRepository,
            IRepository<ContentAnalytics> analyticsRepository,
            IRepository<Notification> notificationRepository,
            IBloggingAgent bloggingAgent,
            ICacheService cacheService)
        {
            _userManager = userManager;
            _blogPostRepository = blogPostRepository;
            _commentRepository = commentRepository;
            _analyticsRepository = analyticsRepository;
            _notificationRepository = notificationRepository;
            _bloggingAgent = bloggingAgent;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cacheKey = $"dashboard_{user.Id}";

            var cachedDashboard = await _cacheService.GetAsync<DashboardViewModel>(cacheKey);
            if (cachedDashboard != null)
            {
                return View(cachedDashboard);
            }

            var dashboard = new DashboardViewModel
            {
                UserStats = await GetUserStatsAsync(user.Id),
                RecentPosts = await GetRecentPostsAsync(user.Id, 5),
                RecentComments = await GetRecentCommentsAsync(user.Id, 5),
                ContentAnalytics = await GetContentAnalyticsAsync(user.Id),
                RecentNotifications = await GetRecentNotificationsAsync(user.Id, 5),
                QuickActions = GetQuickActions(user.Id),
                UpcomingTasks = await GetUpcomingTasksAsync(user.Id)
            };

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, dashboard, TimeSpan.FromMinutes(5));

            return View(dashboard);
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            var user = await _userManager.GetUserAsync(User);
            var cacheKey = $"dashboard_analytics_{user.Id}";

            var cachedAnalytics = await _cacheService.GetAsync<DashboardAnalyticsViewModel>(cacheKey);
            if (cachedAnalytics != null)
            {
                return View(cachedAnalytics);
            }

            var analytics = new DashboardAnalyticsViewModel
            {
                OverviewStats = await GetDetailedAnalyticsAsync(user.Id),
                PostPerformance = await GetPostPerformanceAsync(user.Id),
                AudienceInsights = await GetAudienceInsightsAsync(user.Id),
                ContentTrends = await GetContentTrendsAsync(user.Id),
                TimeRange = "30d" // Default to last 30 days
            };

            // Cache for 15 minutes
            await _cacheService.SetAsync(cacheKey, analytics, TimeSpan.FromMinutes(15));

            return View(analytics);
        }

        [HttpGet]
        public async Task<IActionResult> Content()
        {
            var user = await _userManager.GetUserAsync(User);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);

            var model = new ContentManagementViewModel
            {
                AllPosts = posts.OrderByDescending(p => p.CreatedAt)
                               .Select(p => new BlogPostDto
                               {
                                   Id = p.Id,
                                   Title = p.Title,
                                   Slug = p.Slug,
                                   Excerpt = p.Excerpt,
                                   Author = p.Author,
                                   CreatedAt = p.CreatedAt,
                                   UpdatedAt = p.UpdatedAt,
                                   IsPublished = p.IsPublished,
                                   Tags = p.Tags
                               }).ToList(),
                DraftCount = posts.Count(p => !p.IsPublished),
                PublishedCount = posts.Count(p => p.IsPublished),
                TotalViews = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Views),
                TotalComments = await GetTotalCommentsForUserAsync(user.Id)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> QuickPublish(Guid postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _blogPostRepository.GetByIdAsync(postId);

            if (post == null || post.Author != user.UserName)
            {
                return NotFound();
            }

            post.IsPublished = true;
            post.MarkAsModified();
            await _blogPostRepository.UpdateAsync(post);

            // Clear caches
            await _cacheService.RemoveAsync($"dashboard_{user.Id}");
            await _cacheService.RemoveAsync("blog_index_*");

            return Json(new { success = true, message = "Post published successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> QuickUnpublish(Guid postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _blogPostRepository.GetByIdAsync(postId);

            if (post == null || post.Author != user.UserName)
            {
                return NotFound();
            }

            post.IsPublished = false;
            post.MarkAsModified();
            await _blogPostRepository.UpdateAsync(post);

            // Clear caches
            await _cacheService.RemoveAsync($"dashboard_{user.Id}");
            await _cacheService.RemoveAsync("blog_index_*");

            return Json(new { success = true, message = "Post unpublished successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var user = await _userManager.GetUserAsync(User);
            var stats = await GetUserStatsAsync(user.Id);

            return Json(new
            {
                totalPosts = stats.TotalPosts,
                publishedPosts = stats.PublishedPosts,
                totalViews = stats.TotalViews,
                totalComments = stats.TotalComments,
                recentActivity = stats.RecentActivity?.Take(3).Select(a => new
                {
                    type = a.Type,
                    title = a.Title,
                    timestamp = a.Timestamp
                })
            });
        }

        private async Task<UserStatsDto> GetUserStatsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);

            return new UserStatsDto
            {
                TotalPosts = posts.Count(),
                PublishedPosts = posts.Count(p => p.IsPublished),
                DraftPosts = posts.Count(p => !p.IsPublished),
                TotalComments = await GetTotalCommentsForUserAsync(userId),
                TotalLikes = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Comments), // Using comments as proxy for engagement
                TotalViews = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Views),
                AveragePostRating = 4.5, // Placeholder - would need rating system
                PostsByMonth = GetPostsByMonth(posts),
                TopCategories = GetTopCategories(posts),
                RecentActivity = await GetRecentActivityAsync(userId, 10)
            };
        }

        private async Task<List<BlogPostDto>> GetRecentPostsAsync(string userId, int count)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);

            return posts.OrderByDescending(p => p.CreatedAt)
                       .Take(count)
                       .Select(p => new BlogPostDto
                       {
                           Id = p.Id,
                           Title = p.Title,
                           Slug = p.Slug,
                           Excerpt = p.Excerpt,
                           CreatedAt = p.CreatedAt,
                           IsPublished = p.IsPublished,
                           Tags = p.Tags
                       })
                       .ToList();
        }

        private async Task<List<CommentDto>> GetRecentCommentsAsync(string userId, int count)
        {
            var comments = await _commentRepository.FindAsync(c => c.AuthorId == userId);

            return comments.OrderByDescending(c => c.CreatedAt)
                          .Take(count)
                          .Select(c => new CommentDto
                          {
                              Id = c.Id,
                              Content = c.Content,
                              CreatedAt = c.CreatedAt,
                              IsApproved = c.IsApproved
                          })
                          .ToList();
        }

        private async Task<Dictionary<string, object>> GetContentAnalyticsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);

            return new Dictionary<string, object>
            {
                ["TotalViews"] = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Views),
                ["TotalComments"] = await GetTotalCommentsForUserAsync(userId),
                ["AvgReadTime"] = posts.Where(p => p.Analytics != null).Any() ?
                    (int)posts.Where(p => p.Analytics != null).Average(p => p.Analytics.AverageReadTime) : 0,
                ["TopPerformingPost"] = posts.Where(p => p.Analytics != null)
                    .OrderByDescending(p => p.Analytics.Views)
                    .Select(p => p.Id)
                    .FirstOrDefault()
            };
        }

        private async Task<List<NotificationDto>> GetRecentNotificationsAsync(string userId, int count)
        {
            var notifications = await _notificationRepository.FindAsync(n => n.UserId == userId);

            return notifications.OrderByDescending(n => n.CreatedAt)
                               .Take(count)
                               .Select(n => new NotificationDto
                               {
                                   Id = n.Id,
                                   Title = n.Title,
                                   Message = n.Message,
                                   Type = n.Type,
                                   IsRead = n.IsRead,
                                   CreatedAt = n.CreatedAt,
                                   ActionUrl = n.ActionUrl
                               })
                               .ToList();
        }

        private List<QuickAction> GetQuickActions(string userId)
        {
            return new List<QuickAction>
            {
                new QuickAction
                {
                    Title = "Create New Post",
                    Description = "Generate AI-powered blog content",
                    Icon = "fas fa-plus-circle",
                    Url = "/Blog/Generate",
                    Color = "primary"
                },
                new QuickAction
                {
                    Title = "View Analytics",
                    Description = "Check your content performance",
                    Icon = "fas fa-chart-line",
                    Url = "/Dashboard/Analytics",
                    Color = "success"
                },
                new QuickAction
                {
                    Title = "Manage Content",
                    Description = "Edit and organize your posts",
                    Icon = "fas fa-edit",
                    Url = "/Dashboard/Content",
                    Color = "info"
                },
                new QuickAction
                {
                    Title = "Settings",
                    Description = "Customize your preferences",
                    Icon = "fas fa-cog",
                    Url = "/Settings",
                    Color = "secondary"
                }
            };
        }

        private async Task<List<UpcomingTask>> GetUpcomingTasksAsync(string userId)
        {
            var tasks = new List<UpcomingTask>();
            var user = await _userManager.FindByIdAsync(userId);

            // Check for draft posts
            var draftPosts = await _blogPostRepository.FindAsync(p =>
                p.Author == user.UserName && !p.IsPublished);

            if (draftPosts.Any())
            {
                tasks.Add(new UpcomingTask
                {
                    Title = $"Publish {draftPosts.Count()} draft post(s)",
                    Description = "You have unpublished content ready to share",
                    Priority = "medium",
                    DueDate = DateTime.UtcNow.AddDays(7),
                    ActionUrl = "/Dashboard/Content"
                });
            }

            // Check for posts without analytics
            var postsWithoutAnalytics = await _blogPostRepository.FindAsync(p =>
                p.Author == user.UserName && p.IsPublished &&
                (p.Analytics == null || p.Analytics.Views == 0));

            if (postsWithoutAnalytics.Any())
            {
                tasks.Add(new UpcomingTask
                {
                    Title = "Optimize content performance",
                    Description = $"{postsWithoutAnalytics.Count()} posts could use SEO improvements",
                    Priority = "low",
                    DueDate = DateTime.UtcNow.AddDays(14),
                    ActionUrl = "/Dashboard/Analytics"
                });
            }

            return tasks.OrderBy(t => t.DueDate).Take(3).ToList();
        }

        private async Task<int> GetTotalCommentsForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userPosts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);
            var postIds = userPosts.Select(p => p.Id).ToList();
            var comments = await _commentRepository.FindAsync(c => postIds.Contains(c.BlogPostId));
            return comments.Count();
        }

        private Dictionary<string, int> GetPostsByMonth(IEnumerable<BlogPost> posts)
        {
            return posts.GroupBy(p => p.CreatedAt.ToString("yyyy-MM"))
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, int> GetTopCategories(IEnumerable<BlogPost> posts)
        {
            return posts.SelectMany(p => p.Tags)
                       .GroupBy(tag => tag)
                       .OrderByDescending(g => g.Count())
                       .Take(5)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        private async Task<List<ActivityDto>> GetRecentActivityAsync(string userId, int count)
        {
            var activities = new List<ActivityDto>();
            var user = await _userManager.FindByIdAsync(userId);

            // Recent posts
            var recentPosts = await _blogPostRepository.FindAsync(p =>
                p.Author == user.UserName &&
                p.CreatedAt > DateTime.UtcNow.AddDays(-30));

            foreach (var post in recentPosts.OrderByDescending(p => p.CreatedAt).Take(count / 2))
            {
                activities.Add(new ActivityDto { Type = "post", Title = $"Created post: {post.Title}", Timestamp = post.CreatedAt });
            }

            // Recent comments (on user's posts)
            var userPosts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);
            var postIds = userPosts.Select(p => p.Id).ToList();
            var recentComments = await _commentRepository.FindAsync(c =>
                postIds.Contains(c.BlogPostId) &&
                c.CreatedAt > DateTime.UtcNow.AddDays(-30));

            foreach (var comment in recentComments.OrderByDescending(c => c.CreatedAt).Take(count / 2))
            {
                activities.Add(new ActivityDto { Type = "comment", Title = $"New comment received", Timestamp = comment.CreatedAt });
            }

            return activities.OrderByDescending(a => a.Timestamp).Take(count).ToList();
        }

        private async Task<Dictionary<string, object>> GetDetailedAnalyticsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);

            return new Dictionary<string, object>
            {
                ["totalPosts"] = posts.Count(),
                ["publishedPosts"] = posts.Count(p => p.IsPublished),
                ["totalViews"] = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Views),
                ["totalComments"] = await GetTotalCommentsForUserAsync(userId),
                ["avgViewsPerPost"] = posts.Where(p => p.Analytics != null).Any() ?
                    posts.Where(p => p.Analytics != null).Average(p => p.Analytics.Views) : 0,
                ["topPerformingPost"] = posts.Where(p => p.Analytics != null)
                    .OrderByDescending(p => p.Analytics.Views)
                    .Select(p => p.Title)
                    .FirstOrDefault() ?? "No data"
            };
        }

        private async Task<List<Dictionary<string, object>>> GetPostPerformanceAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName && p.IsPublished);

            return posts.OrderByDescending(p => p.CreatedAt)
                       .Take(10)
                       .Select(p => new Dictionary<string, object>
                       {
                           ["title"] = p.Title,
                           ["views"] = p.Analytics?.Views ?? 0,
                           ["comments"] = p.Analytics?.Comments ?? 0,
                           ["publishedDate"] = p.CreatedAt.ToString("yyyy-MM-dd"),
                           ["slug"] = p.Slug
                       })
                       .ToList();
        }

        private async Task<Dictionary<string, object>> GetAudienceInsightsAsync(string userId)
        {
            // Placeholder - would need actual analytics data
            return new Dictionary<string, object>
            {
                ["topCountries"] = new[] { "United States", "United Kingdom", "Canada" },
                ["topReferrers"] = new[] { "Google", "Direct", "Social Media" },
                ["deviceTypes"] = new Dictionary<string, double>
                {
                    ["Desktop"] = 65.5,
                    ["Mobile"] = 28.3,
                    ["Tablet"] = 6.2
                },
                ["peakHours"] = new[] { "14:00", "15:00", "16:00" }
            };
        }

        private async Task<Dictionary<string, object>> GetContentTrendsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = await _blogPostRepository.FindAsync(p => p.Author == user.UserName);

            var last30Days = DateTime.UtcNow.AddDays(-30);
            var recentPosts = posts.Where(p => p.CreatedAt > last30Days);

            return new Dictionary<string, object>
            {
                ["postsThisMonth"] = recentPosts.Count(),
                ["viewsThisMonth"] = recentPosts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Views),
                ["growthRate"] = CalculateGrowthRate(posts),
                ["popularTopics"] = GetTopCategories(recentPosts),
                ["engagementRate"] = CalculateEngagementRate(recentPosts)
            };
        }

        private double CalculateGrowthRate(IEnumerable<BlogPost> posts)
        {
            var thisMonth = posts.Count(p => p.CreatedAt > DateTime.UtcNow.AddDays(-30));
            var lastMonth = posts.Count(p =>
                p.CreatedAt > DateTime.UtcNow.AddDays(-60) &&
                p.CreatedAt <= DateTime.UtcNow.AddDays(-30));

            if (lastMonth == 0) return thisMonth > 0 ? 100.0 : 0.0;

            return ((double)(thisMonth - lastMonth) / lastMonth) * 100.0;
        }

        private double CalculateEngagementRate(IEnumerable<BlogPost> posts)
        {
            var totalViews = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Views);
            var totalComments = posts.Where(p => p.Analytics != null).Sum(p => p.Analytics.Comments);

            if (totalViews == 0) return 0.0;

            return ((double)totalComments / totalViews) * 100.0;
        }
    }

}