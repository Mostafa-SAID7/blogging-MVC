using System;
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
        private readonly IRepository<ContentAnalytics> _analyticsRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IRepository<ContentAnalytics> analyticsRepository,
            IRepository<BlogPost> blogPostRepository,
            ICacheService cacheService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsRepository = analyticsRepository;
            _blogPostRepository = blogPostRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("analytics/post/{id}")]
        public async Task<IActionResult> PostAnalytics(Guid id)
        {
            var analytics = await _analyticsRepository.GetByIdAsync(id);
            if (analytics == null)
            {
                return NotFound();
            }

            var model = new ContentAnalyticsViewModel
            {
                BlogPostId = analytics.BlogPostId,
                Views = analytics.Views,
                UniqueViews = analytics.UniqueViews,
                Shares = analytics.Shares,
                Comments = analytics.Comments,
                AverageReadTime = analytics.AverageReadTime,
                BounceRate = analytics.BounceRate
            };

            return Json(model);
        }

        [HttpPost]
        [Route("api/analytics/{postId}/record-view")]
        public async Task<IActionResult> RecordView(Guid postId)
        {
            try
            {
                var analytics = await _analyticsRepository.GetByIdAsync(postId);
                if (analytics == null)
                {
                    analytics = new ContentAnalytics { BlogPostId = postId };
                    await _analyticsRepository.AddAsync(analytics);
                }

                analytics.Views++;
                await _analyticsRepository.UpdateAsync(analytics);

                // Invalidate caches related to this post
                await _cacheService.RemoveAsync($"blog_details_*_{postId}");

                return Json(new { success = true, views = analytics.Views });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record view for post {PostId}", postId);
                return StatusCode(500);
            }
        }
    }
}
