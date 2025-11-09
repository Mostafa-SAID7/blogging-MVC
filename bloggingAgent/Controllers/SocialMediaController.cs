using System;
using System.Threading.Tasks;
using BloggingAgent.Services.SocialMedia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers
{
    [Authorize]
    public class SocialMediaController : Controller
    {
        private readonly ISocialMediaService _socialMediaService;
        private readonly ILogger<SocialMediaController> _logger;

        public SocialMediaController(
            ISocialMediaService socialMediaService,
            ILogger<SocialMediaController> logger)
        {
            _socialMediaService = socialMediaService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Author,Editor,Administrator")]
        public async Task<IActionResult> SharePost(int postId, string platforms, string customMessage = null)
        {
            try
            {
                // In a real implementation, you'd fetch the post from the database
                // For demo purposes, we'll use placeholder content
                var postTitle = "Sample Blog Post";
                var postUrl = $"{Request.Scheme}://{Request.Host}/blog/{postId}";
                var message = customMessage ?? $"{postTitle} {postUrl}";

                var platformList = platforms.Split(',');
                var results = new System.Collections.Generic.Dictionary<string, bool>();

                foreach (var platform in platformList)
                {
                    var success = platform.Trim().ToLower() switch
                    {
                        "twitter" => await _socialMediaService.PostToTwitterAsync(message),
                        "linkedin" => await _socialMediaService.PostToLinkedInAsync(message),
                        "facebook" => await _socialMediaService.PostToFacebookAsync(message),
                        _ => false
                    };

                    results[platform] = success;
                }

                var successfulPosts = results.Count(r => r.Value);
                var totalPosts = results.Count;

                if (successfulPosts > 0)
                {
                    TempData["Success"] = $"Successfully shared to {successfulPosts} of {totalPosts} platforms.";
                }
                else
                {
                    TempData["Error"] = "Failed to share to any platforms.";
                }

                _logger.LogInformation("Post {PostId} shared to social media: {Results}",
                    postId, string.Join(", ", results.Select(r => $"{r.Key}: {r.Value}")));

                return RedirectToAction("Details", "Blog", new { id = postId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing post {PostId} to social media", postId);
                TempData["Error"] = "An error occurred while sharing to social media.";
                return RedirectToAction("Details", "Blog", new { id = postId });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Author,Editor,Administrator")]
        public async Task<IActionResult> SchedulePost(string platforms, string content, DateTime scheduledTime, string imageUrl = null)
        {
            try
            {
                var platformList = platforms.Split(',');
                var results = new System.Collections.Generic.Dictionary<string, bool>();

                foreach (var platform in platformList)
                {
                    var success = await _socialMediaService.SchedulePostAsync(
                        platform.Trim(), content, scheduledTime, imageUrl);
                    results[platform] = success;
                }

                var successfulSchedules = results.Count(r => r.Value);
                var totalSchedules = results.Count;

                if (successfulSchedules > 0)
                {
                    TempData["Success"] = $"Successfully scheduled {successfulSchedules} of {totalSchedules} posts.";
                }
                else
                {
                    TempData["Error"] = "Failed to schedule any posts.";
                }

                _logger.LogInformation("Posts scheduled: {Results}",
                    string.Join(", ", results.Select(r => $"{r.Key}: {r.Value}")));

                return RedirectToAction("Index", "Blog");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling social media posts");
                TempData["Error"] = "An error occurred while scheduling posts.";
                return RedirectToAction("Index", "Blog");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Settings()
        {
            var configuredPlatforms = _socialMediaService.GetConfiguredPlatforms();
            ViewBag.ConfiguredPlatforms = configuredPlatforms;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> TestConnection(string platform)
        {
            try
            {
                var testMessage = $"Test post from BloggingAgent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                var success = platform.ToLower() switch
                {
                    "twitter" => await _socialMediaService.PostToTwitterAsync(testMessage),
                    "linkedin" => await _socialMediaService.PostToLinkedInAsync(testMessage),
                    "facebook" => await _socialMediaService.PostToFacebookAsync(testMessage),
                    _ => false
                };

                if (success)
                {
                    TempData["Success"] = $"{platform} connection test successful!";
                }
                else
                {
                    TempData["Error"] = $"{platform} connection test failed. Check your configuration.";
                }

                _logger.LogInformation("{Platform} connection test: {Success}", platform, success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing {Platform} connection", platform);
                TempData["Error"] = $"Error testing {platform} connection: {ex.Message}";
            }

            return RedirectToAction("Settings");
        }

        [HttpGet]
        public IActionResult GetConfiguredPlatforms()
        {
            var platforms = _socialMediaService.GetConfiguredPlatforms();
            return Json(new { platforms });
        }
    }
}