using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.SocialMedia
{
    public class SocialMediaService : ISocialMediaService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SocialMediaService> _logger;
        private readonly HttpClient _httpClient;

        public SocialMediaService(
            IConfiguration configuration,
            ILogger<SocialMediaService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> PostToTwitterAsync(string content, string imageUrl = null)
        {
            if (!IsPlatformConfigured("Twitter"))
            {
                _logger.LogWarning("Twitter is not configured");
                return false;
            }

            try
            {
                // Twitter API v2 implementation
                var apiKey = _configuration["SocialMedia:Twitter:ApiKey"];
                var apiSecret = _configuration["SocialMedia:Twitter:ApiSecret"];
                var accessToken = _configuration["SocialMedia:Twitter:AccessToken"];
                var accessTokenSecret = _configuration["SocialMedia:Twitter:AccessTokenSecret"];

                // In a real implementation, you'd use a Twitter SDK or implement OAuth 1.0a
                // For demo purposes, we'll simulate the API call
                var tweetData = new
                {
                    text = content.Length > 280 ? content.Substring(0, 277) + "..." : content
                };

                // Simulate API call
                await Task.Delay(100); // Simulate network delay

                _logger.LogInformation("Successfully posted to Twitter");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to post to Twitter");
                return false;
            }
        }

        public async Task<bool> PostToLinkedInAsync(string content, string imageUrl = null)
        {
            if (!IsPlatformConfigured("LinkedIn"))
            {
                _logger.LogWarning("LinkedIn is not configured");
                return false;
            }

            try
            {
                var accessToken = _configuration["SocialMedia:LinkedIn:AccessToken"];
                var personId = _configuration["SocialMedia:LinkedIn:PersonId"];

                var postData = new Dictionary<string, object>
                {
                    ["author"] = $"urn:li:person:{personId}",
                    ["lifecycleState"] = "PUBLISHED",
                    ["specificContent"] = new Dictionary<string, object>
                    {
                        ["com.linkedin.ugc.ShareContent"] = new Dictionary<string, object>
                        {
                            ["shareCommentary"] = new Dictionary<string, string>
                            {
                                ["text"] = content
                            },
                            ["shareMediaCategory"] = imageUrl != null ? "IMAGE" : "NONE"
                        }
                    },
                    ["visibility"] = new Dictionary<string, string>
                    {
                        ["com.linkedin.ugc.MemberNetworkVisibility"] = "PUBLIC"
                    }
                };

                var jsonContent = JsonSerializer.Serialize(postData);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.linkedin.com/v2/ugcPosts")
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully posted to LinkedIn");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to post to LinkedIn");
                return false;
            }
        }

        public async Task<bool> PostToFacebookAsync(string content, string imageUrl = null)
        {
            if (!IsPlatformConfigured("Facebook"))
            {
                _logger.LogWarning("Facebook is not configured");
                return false;
            }

            try
            {
                var pageAccessToken = _configuration["SocialMedia:Facebook:PageAccessToken"];
                var pageId = _configuration["SocialMedia:Facebook:PageId"];

                var postData = new Dictionary<string, string>
                {
                    ["message"] = content,
                    ["access_token"] = pageAccessToken
                };

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    postData["link"] = imageUrl;
                }

                var formContent = new FormUrlEncodedContent(postData);
                var response = await _httpClient.PostAsync($"https://graph.facebook.com/v18.0/{pageId}/feed", formContent);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully posted to Facebook");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to post to Facebook");
                return false;
            }
        }

        public async Task<bool> SchedulePostAsync(string platform, string content, DateTime scheduledTime, string imageUrl = null)
        {
            // In a real implementation, you'd store this in a database and use a background job
            // For demo purposes, we'll just log it
            _logger.LogInformation("Scheduled post to {Platform} at {ScheduledTime}: {Content}",
                platform, scheduledTime, content);

            // Simulate scheduling
            var delay = scheduledTime - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);

                switch (platform.ToLower())
                {
                    case "twitter":
                        return await PostToTwitterAsync(content, imageUrl);
                    case "linkedin":
                        return await PostToLinkedInAsync(content, imageUrl);
                    case "facebook":
                        return await PostToFacebookAsync(content, imageUrl);
                    default:
                        _logger.LogWarning("Unknown platform: {Platform}", platform);
                        return false;
                }
            }

            return false;
        }

        public bool IsPlatformConfigured(string platform)
        {
            return platform.ToLower() switch
            {
                "twitter" => !string.IsNullOrEmpty(_configuration["SocialMedia:Twitter:ApiKey"]),
                "linkedin" => !string.IsNullOrEmpty(_configuration["SocialMedia:LinkedIn:AccessToken"]),
                "facebook" => !string.IsNullOrEmpty(_configuration["SocialMedia:Facebook:PageAccessToken"]),
                _ => false
            };
        }

        public List<string> GetConfiguredPlatforms()
        {
            var platforms = new List<string>();

            if (IsPlatformConfigured("Twitter")) platforms.Add("Twitter");
            if (IsPlatformConfigured("LinkedIn")) platforms.Add("LinkedIn");
            if (IsPlatformConfigured("Facebook")) platforms.Add("Facebook");

            return platforms;
        }
    }
}