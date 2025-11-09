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
        private readonly ILogger<BloggingAgent> _logger;
        private readonly AgentSettings _settings;

        public BloggingAgent(
            ILlmConnector llmConnector,
            ISeoService seoService,
            IMemoryService memoryService,
            IContentFormatter contentFormatter,
            ILogger<BloggingAgent> logger,
            IOptions<AgentSettings> settings)
        {
            _llmConnector = llmConnector;
            _seoService = seoService;
            _memoryService = memoryService;
            _contentFormatter = contentFormatter;
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

            _logger.LogInformation("Successfully generated blog post: {Title}", post.Title);
            return post;
        }

        public async Task<SeoAnalysisResult> AnalyzePostAsync(string content, string title = null)
        {
            return await _seoService.AnalyzeContentAsync(content, title);
        }

        public async Task<string> OptimizeContentAsync(string content, string[] keywords)
        {
            var prompt = $"Optimize the following content for SEO with these keywords: {string.Join(", ", keywords)}\n\n{content}";
            return await _llmConnector.GenerateContentAsync(prompt, content.Length + 200);
        }

        public async Task<string> GenerateExcerptAsync(string content)
        {
            return await _contentFormatter.ExtractExcerptAsync(content);
        }

        public async Task<string[]> SuggestTagsAsync(string content)
        {
            var keywords = await _seoService.SuggestKeywordsAsync(content, 8);
            return keywords.Where(k => k.Length > 2).ToArray();
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

            // If no memory, assume relevant and store for future
            await _memoryService.StoreAsync(memoryKey, "true", "topic_relevance");
            return true;
        }

        private async Task<string> GenerateContentAsync(GeneratePostRequest request)
        {
            var prompt = BuildContentPrompt(request);
            var content = await _llmConnector.GenerateContentAsync(prompt, request.TargetWordCount * 6); // Rough token estimate

            // Ensure word count
            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < request.TargetWordCount * 0.8) // Allow 20% variance
            {
                var additionalPrompt = $"Expand the following content to reach approximately {request.TargetWordCount} words:\n\n{content}";
                content = await _llmConnector.GenerateContentAsync(additionalPrompt, request.TargetWordCount * 6);
            }

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
    }
}