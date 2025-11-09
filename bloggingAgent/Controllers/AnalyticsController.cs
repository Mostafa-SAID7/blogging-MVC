using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<ContentAnalytics> _analyticsRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IRepository<BlogPost> blogPostRepository,
            IRepository<ContentAnalytics> analyticsRepository,
            ICacheService cacheService,
            ILogger<AnalyticsController> logger)
        {
            _blogPostRepository = blogPostRepository;
            _analyticsRepository = analyticsRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            const string cacheKey = "analytics_index";
            var cachedModel = await _cacheService.GetAsync<AnalyticsViewModel>(cacheKey);
            if (cachedModel != null)
                return View(cachedModel);

            var posts = await _blogPostRepository.GetAllAsync();
            var analytics = await _analyticsRepository.GetAllAsync();

            var postAnalytics = analytics.Join(posts,
                a => a.BlogPostId,
                p => p.Id,
                (a, p) => new { Analytics = a, Post = p })
                .Select(x => x.Analytics)
                .ToList();

            var model = new AnalyticsViewModel
            {
                PostAnalytics = postAnalytics,
                TotalViews = postAnalytics.Sum(a => a.Views),
                TotalPosts = posts.Count(p => p.IsPublished),
                AverageReadTime = postAnalytics.Any() ? postAnalytics.Average(a => a.AverageReadTime) : 0,
                TopTags = GetTopTags(posts),
                TrafficSources = GetTrafficSources(postAnalytics),
                PerformanceMetrics = CalculatePerformanceMetrics(postAnalytics)
            };

            // Cache for 15 minutes
            await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(15));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PostDetails(int id)
        {
            var cacheKey = $"analytics_post_{id}";
            var cachedAnalytics = await _cacheService.GetAsync<ContentAnalytics>(cacheKey);
            if (cachedAnalytics != null)
                return Json(cachedAnalytics);

            var analytics = await _analyticsRepository.GetByIdAsync(id);
            if (analytics == null)
                return NotFound();

            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, analytics, TimeSpan.FromMinutes(10));

            return Json(analytics);
        }

        [HttpGet]
        public async Task<IActionResult> ExportData(string format = "json")
        {
            var posts = await _blogPostRepository.GetAllAsync();
            var analytics = await _analyticsRepository.GetAllAsync();

            var data = new
            {
                Posts = posts.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.Author,
                    p.CreatedAt,
                    p.IsPublished,
                    Tags = string.Join(", ", p.Tags),
                    Analytics = analytics.FirstOrDefault(a => a.BlogPostId == p.Id)
                }),
                Summary = new
                {
                    TotalPosts = posts.Count,
                    PublishedPosts = posts.Count(p => p.IsPublished),
                    TotalViews = analytics.Sum(a => a.Views),
                    AverageReadTime = analytics.Any() ? analytics.Average(a => a.AverageReadTime) : 0
                }
            };

            if (format.ToLower() == "csv")
            {
                // Return CSV format
                var csv = GenerateCsv(data.Posts);
                return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "blog_analytics.csv");
            }

            return Json(data);
        }

        private Dictionary<string, int> GetTopTags(System.Collections.Generic.IEnumerable<BlogPost> posts)
        {
            return posts.Where(p => p.IsPublished)
                       .SelectMany(p => p.Tags)
                       .GroupBy(t => t)
                       .OrderByDescending(g => g.Count())
                       .Take(10)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        private System.Collections.Generic.List<KeyValuePair<string, int>> GetTrafficSources(System.Collections.Generic.IEnumerable<ContentAnalytics> analytics)
        {
            return analytics.SelectMany(a => a.TrafficSources)
                           .GroupBy(ts => ts.Key)
                           .Select(g => new KeyValuePair<string, int>(g.Key, g.Sum(ts => ts.Value)))
                           .OrderByDescending(ts => ts.Value)
                           .ToList();
        }

        private Dictionary<string, double> CalculatePerformanceMetrics(System.Collections.Generic.IEnumerable<ContentAnalytics> analytics)
        {
            var metrics = new Dictionary<string, double>();

            if (!analytics.Any())
                return metrics;

            metrics["AverageViews"] = analytics.Average(a => a.Views);
            metrics["AverageBounceRate"] = analytics.Average(a => a.BounceRate);
            metrics["AverageReadTime"] = analytics.Average(a => a.AverageReadTime);
            metrics["TotalShares"] = analytics.Sum(a => a.Shares);
            metrics["TotalComments"] = analytics.Sum(a => a.Comments);

            return metrics;
        }

        private string GenerateCsv(System.Collections.Generic.IEnumerable<dynamic> posts)
        {
            var csv = "ID,Title,Slug,Author,CreatedAt,IsPublished,Tags,Views,UniqueViews,Shares,Comments,AverageReadTime,BounceRate\n";

            foreach (var post in posts)
            {
                var analytics = post.Analytics;
                csv += $"{post.Id},";
                csv += $"\"{post.Title?.Replace("\"", "\"\"")}\",";
                csv += $"{post.Slug},";
                csv += $"\"{post.Author?.Replace("\"", "\"\"")}\",";
                csv += $"{post.CreatedAt:yyyy-MM-dd HH:mm:ss},";
                csv += $"{post.IsPublished},";
                csv += $"\"{post.Tags?.Replace("\"", "\"\"")}\",";

                if (analytics != null)
                {
                    csv += $"{analytics.Views},";
                    csv += $"{analytics.UniqueViews},";
                    csv += $"{analytics.Shares},";
                    csv += $"{analytics.Comments},";
                    csv += $"{analytics.AverageReadTime},";
                    csv += $"{analytics.BounceRate}";
                }
                else
                {
                    csv += "0,0,0,0,0,0";
                }

                csv += "\n";
            }

            return csv;
        }
    }
}