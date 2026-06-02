using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.Email;
using BloggingAgent.Services.SocialMedia;
using BloggingAgent.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Agents
{
    public class BloggingAgent : IBloggingAgent
    {
        private readonly ILlmConnector _llmConnector;
        private readonly ISeoService _seoService;
        private readonly IMemoryService _memoryService;
        private readonly IContentFormatter _contentFormatter;
        private readonly ICacheService _cacheService;
        private readonly ISocialMediaService _socialMediaService;
        private readonly IEmailService _emailService;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly ILogger<BloggingAgent> _logger;
        private readonly AgentSettings _settings;

        public BloggingAgent(
            ILlmConnector llmConnector,
            ISeoService seoService,
            IMemoryService memoryService,
            IContentFormatter contentFormatter,
            ICacheService cacheService,
            ISocialMediaService socialMediaService,
            IEmailService emailService,
            IRepository<BlogPost> blogPostRepository,
            ILogger<BloggingAgent> logger,
            IOptions<AgentSettings> settings)
        {
            _llmConnector = llmConnector;
            _seoService = seoService;
            _memoryService = memoryService;
            _contentFormatter = contentFormatter;
            _cacheService = cacheService;
            _socialMediaService = socialMediaService;
            _emailService = emailService;
            _blogPostRepository = blogPostRepository;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<BlogPostDto> GeneratePostAsync(GeneratePostRequest request)
        {
            _logger.LogInformation("Generating blog post for topic: {Topic}", request.Topic);

            // Check if topic is relevant based on memory
            if (!await IsTopicRelevantAsync(request.Topic))
            {
                _logger.LogWarning("Topic {Topic} deemed not relevant", request.Topic);
                throw new InvalidOperationException("Topic is not relevant for this blog");
            }

            // Generate content using LLM
            var content = await GenerateContentAsync(request);

            // Optimize for SEO
            if (request.Keywords?.Any() == true)
            {
                content = await OptimizeContentAsync(content, request.Keywords.Split(','));
            }

            // Format content
            var formattedContent = await _contentFormatter.FormatAsHtmlAsync(content);
            var excerpt = await _contentFormatter.ExtractExcerptAsync(content);

            // Generate SEO metadata
            var seoAnalysis = await AnalyzePostAsync(content, request.Topic);

            // Suggest tags
            var suggestedTags = await SuggestTagsAsync(content);
            var allTags = request.Tags.Union(suggestedTags).Distinct().ToList();

            // Add default tags from settings
            if (_settings.DefaultTags?.Any() == true)
            {
                allTags.AddRange(_settings.DefaultTags.Where(tag => !allTags.Contains(tag)));
            }

            var post = new BlogPostDto
            {
                Title = request.Topic,
                Slug = GenerateSlug(request.Topic),
                Content = formattedContent,
                Excerpt = excerpt,
                Author = _settings.DefaultAuthor ?? "AI Assistant",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = _settings.AutoPublish,
                Tags = allTags
            };

            // Store in memory for future reference
            await _memoryService.StoreAsync($"post_{post.Slug}", content, "posts");

            // Cache the generated post
            await _cacheService.SetAsync($"generated_post_{post.Slug}", post, TimeSpan.FromHours(1));

            _logger.LogInformation("Successfully generated blog post: {Title}", post.Title);
            return post;
        }

        public async Task<SeoAnalysisResult> AnalyzePostAsync(string content, string title = null)
        {
            var cacheKey = $"seo_analysis_{GenerateSlug(title ?? "untitled")}";
            var cachedAnalysis = await _cacheService.GetAsync<SeoAnalysisResult>(cacheKey);

            if (cachedAnalysis != null)
            {
                return cachedAnalysis;
            }

            var analysis = await _seoService.AnalyzeContentAsync(content, title);

            // Cache for 30 minutes
            await _cacheService.SetAsync(cacheKey, analysis, TimeSpan.FromMinutes(30));

            return analysis;
        }

        public async Task<string> OptimizeContentAsync(string content, string[] keywords)
        {
            var prompt = $"Optimize the following content for SEO with these keywords: {string.Join(", ", keywords)}\n\n{content}";
            var optimizedContent = await _llmConnector.GenerateContentAsync(prompt, content.Length + 200);

            // Cache the optimization
            var cacheKey = $"optimized_content_{GenerateSlug(keywords.FirstOrDefault() ?? "general")}";
            await _cacheService.SetAsync(cacheKey, optimizedContent, TimeSpan.FromHours(1));

            return optimizedContent;
        }

        public async Task<string> GenerateExcerptAsync(string content)
        {
            return await _contentFormatter.ExtractExcerptAsync(content);
        }

        public async Task<string[]> SuggestTagsAsync(string content)
        {
            var cacheKey = $"suggested_tags_{content.GetHashCode()}";
            var cachedTags = await _cacheService.GetAsync<string[]>(cacheKey);

            if (cachedTags != null)
            {
                return cachedTags;
            }

            var keywords = await _seoService.SuggestKeywordsAsync(content, 8);
            var tags = keywords.Where(k => k.Length > 2).ToArray();

            // Cache for 1 hour
            await _cacheService.SetAsync(cacheKey, tags, TimeSpan.FromHours(1));

            return tags;
        }

        public async Task<bool> IsTopicRelevantAsync(string topic)
        {
            // Check memory for topic relevance
            var memoryKey = $"topic_relevance_{topic.ToLower()}";
            var relevance = await _memoryService.RetrieveAsync(memoryKey);

            if (!string.IsNullOrEmpty(relevance))
            {
                return bool.Parse(relevance);
            }

            // If no memory, check against existing posts for relevance
            var existingPosts = await _blogPostRepository.GetAllAsync();
            var relevantPosts = existingPosts.Where(p =>
                p.Title.Contains(topic, StringComparison.OrdinalIgnoreCase) ||
                p.Content.Contains(topic, StringComparison.OrdinalIgnoreCase) ||
                p.Tags.Any(tag => tag.Contains(topic, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            // Consider relevant if we have related content or if it's a new topic
            var isRelevant = relevantPosts.Any() || topic.Length > 3; // Basic relevance check

            // Store in memory for future reference
            await _memoryService.StoreAsync(memoryKey, isRelevant.ToString(), "topic_relevance");

            return isRelevant;
        }

        public async Task<BlogPostDto> PublishPostAsync(BlogPostDto post)
        {
            // Save to database
            var domainPost = MapToDomain(post);
            await _blogPostRepository.AddAsync(domainPost);

            // Update the DTO with the database ID
            post.Id = domainPost.Id;

            // Clear related caches
            await _cacheService.RemoveAsync("blog_index_*");
            await _cacheService.RemoveAsync("blog_details_*");

            // Auto-share to social media if enabled
            if (_settings.AutoPublish && _socialMediaService.GetConfiguredPlatforms().Any())
            {
                var shareMessage = $"{post.Title} - {post.Excerpt}";
                var shareUrl = $"/blog/{post.Slug}";

                foreach (var platform in _socialMediaService.GetConfiguredPlatforms())
                {
                    try
                    {
                        var success = platform.ToLower() switch
                        {
                            "twitter" => await _socialMediaService.PostToTwitterAsync($"{shareMessage} {shareUrl}"),
                            "linkedin" => await _socialMediaService.PostToLinkedInAsync($"{shareMessage} {shareUrl}"),
                            "facebook" => await _socialMediaService.PostToFacebookAsync($"{shareMessage} {shareUrl}"),
                            _ => false
                        };

                        if (success)
                        {
                            _logger.LogInformation("Auto-shared post {PostId} to {Platform}", post.Id, platform);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to auto-share post {PostId} to {Platform}", post.Id, platform);
                    }
                }
            }

            // Send notification email if enabled
            if (_settings.AutoPublish)
            {
                await SendPostPublishedNotificationAsync(post);
            }

            _logger.LogInformation("Successfully published blog post: {Title} (ID: {Id})", post.Title, post.Id);
            return post;
        }

        public async Task<BlogPostDto> UpdatePostAsync(Guid postId, BlogPostDto updatedPost)
        {
            var existingPost = await _blogPostRepository.GetByIdAsync(postId);
            if (existingPost == null)
            {
                throw new KeyNotFoundException($"Post with ID {postId} not found");
            }

            // Update properties
            existingPost.Title = updatedPost.Title;
            existingPost.Slug = GenerateSlug(updatedPost.Title);
            existingPost.Content = updatedPost.Content;
            existingPost.Excerpt = updatedPost.Excerpt;
            existingPost.Tags = updatedPost.Tags;
            existingPost.IsPublished = updatedPost.IsPublished;
            existingPost.UpdatedAt = DateTime.UtcNow;

            await _blogPostRepository.UpdateAsync(existingPost);

            // Clear caches
            await _cacheService.RemoveAsync($"blog_details_{existingPost.Slug}");
            await _cacheService.RemoveAsync("blog_index_*");

            // Update memory
            await _memoryService.StoreAsync($"post_{existingPost.Slug}", existingPost.Content, "posts");

            _logger.LogInformation("Successfully updated blog post: {Title} (ID: {Id})", existingPost.Title, existingPost.Id);
            return MapToDto(existingPost);
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _blogPostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return false;
            }

            await _blogPostRepository.DeleteAsync(postId);

            // Clear caches
            await _cacheService.RemoveAsync($"blog_details_{post.Slug}");
            await _cacheService.RemoveAsync("blog_index_*");

            // Remove from memory
            await _memoryService.StoreAsync($"post_{post.Slug}", null, "deleted_posts");

            _logger.LogInformation("Successfully deleted blog post: {Title} (ID: {Id})", post.Title, post.Id);
            return true;
        }

        public async Task<List<BlogPostDto>> GetRelatedPostsAsync(Guid postId, int count = 5)
        {
            var post = await _blogPostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return new List<BlogPostDto>();
            }

            var allPosts = await _blogPostRepository.GetAllAsync();
            var relatedPosts = allPosts.Where(p =>
                p.Id != postId &&
                p.IsPublished &&
                p.Tags.Intersect(post.Tags).Any()
            )
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(MapToDto)
            .ToList();

            return relatedPosts;
        }

        public async Task<Dictionary<string, int>> GetContentAnalyticsAsync()
        {
            var posts = await _blogPostRepository.GetAllAsync();
            var analytics = new Dictionary<string, int>
            {
                ["TotalPosts"] = posts.Count(),
                ["PublishedPosts"] = posts.Count(p => p.IsPublished),
                ["DraftPosts"] = posts.Count(p => !p.IsPublished),
                ["TotalTags"] = posts.SelectMany(p => p.Tags).Distinct().Count(),
                ["AverageWordCount"] = posts.Any() ? (int)posts.Average(p => p.Content.Length / 5.0) : 0
            };

            return analytics;
        }

        private async Task<string> GenerateContentAsync(GeneratePostRequest request)
        {
            var cacheKey = $"generated_content_{GenerateSlug(request.Topic)}_{request.TargetWordCount}";
            var cachedContent = await _cacheService.GetAsync<string>(cacheKey);

            if (cachedContent != null)
            {
                return cachedContent;
            }

            var prompt = BuildContentPrompt(request);
            var content = await _llmConnector.GenerateContentAsync(prompt, request.TargetWordCount * 6); // Rough token estimate

            // Ensure word count
            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < request.TargetWordCount * 0.8) // Allow 20% variance
            {
                var additionalPrompt = $"Expand the following content to reach approximately {request.TargetWordCount} words:\n\n{content}";
                content = await _llmConnector.GenerateContentAsync(additionalPrompt, request.TargetWordCount * 6);
            }

            // Cache the generated content
            await _cacheService.SetAsync(cacheKey, content, TimeSpan.FromHours(2));

            return content;
        }

        private string BuildContentPrompt(GeneratePostRequest request)
        {
            var prompt = $"Write a comprehensive blog post about: {request.Topic}\n\n";

            if (!string.IsNullOrEmpty(request.Keywords))
                prompt += $"Keywords to include: {request.Keywords}\n";

            if (!string.IsNullOrEmpty(request.Tone))
                prompt += $"Tone: {request.Tone}\n";

            if (!string.IsNullOrEmpty(request.TargetAudience))
                prompt += $"Target audience: {request.TargetAudience}\n";

            prompt += $"Target word count: {request.TargetWordCount}\n\n";
            prompt += "Structure the post with:\n";
            prompt += "- An engaging introduction\n";
            prompt += "- Main content sections with descriptive headings\n";
            prompt += "- Practical examples or case studies\n";
            prompt += "- A conclusion with key takeaways\n\n";
            prompt += "Make it informative, well-structured, and engaging.";

            return prompt;
        }

        private async Task SendPostPublishedNotificationAsync(BlogPostDto post)
        {
            try
            {
                // In a real implementation, you'd get subscribers from a database
                // For now, we'll just log the notification
                _logger.LogInformation("Post published notification would be sent for: {Title}", post.Title);

                // Example email content
                var subject = $"New Post Published: {post.Title}";
                var body = $@"
                <h2>New Blog Post Published!</h2>
                <h3>{post.Title}</h3>
                <p>{post.Excerpt}</p>
                <p><a href='/blog/{post.Slug}'>Read the full post</a></p>
                ";

                // await _emailService.SendEmailAsync(subscriberEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send post published notification");
            }
        }

        private string GenerateSlug(string title)
        {
            return title.ToLower()
                       .Replace(" ", "-")
                       .Replace(".", "")
                       .Replace(",", "")
                       .Replace("?", "")
                       .Replace("!", "")
                       .Replace(":", "")
                       .Replace(";", "")
                       .Replace("\"", "")
                       .Replace("'", "");
        }

        private BlogPost MapToDomain(BlogPostDto dto)
        {
            return new BlogPost
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Content = dto.Content,
                Excerpt = dto.Excerpt,
                Author = dto.Author,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                IsPublished = dto.IsPublished,
                Tags = dto.Tags,
                SeoMetadata = new SeoMetadata(), // Will be populated by SEO service
                Analytics = new ContentAnalytics() // Will be populated by analytics service
            };
        }

        private BlogPostDto MapToDto(BlogPost post)
        {
            return new BlogPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                Author = post.Author,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsPublished = post.IsPublished,
                Tags = post.Tags
            };
        }
    }
}