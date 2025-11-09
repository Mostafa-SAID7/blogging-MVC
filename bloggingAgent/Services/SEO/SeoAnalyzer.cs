using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.SEO
{
    public class SeoAnalyzer
    {
        private readonly ISeoService _seoService;
        private readonly ILogger<SeoAnalyzer> _logger;

        public SeoAnalyzer(ISeoService seoService, ILogger<SeoAnalyzer> logger)
        {
            _seoService = seoService;
            _logger = logger;
        }

        public async Task<SeoMetadata> GenerateSeoMetadataAsync(string title, string content, List<string> keywords = null)
        {
            var metadata = new SeoMetadata
            {
                Title = title,
                Description = await _seoService.GenerateMetaDescriptionAsync(content),
                Keywords = keywords != null ? string.Join(", ", keywords) : null,
                CanonicalUrl = GenerateCanonicalUrl(title),
                OgTitle = title,
                OgDescription = await _seoService.GenerateMetaDescriptionAsync(content),
                OgImage = null, // Would be set by content processor
                TwitterCard = "summary_large_image"
            };

            // Generate structured data
            metadata.StructuredData = new Dictionary<string, string>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "BlogPosting",
                ["headline"] = title,
                ["description"] = metadata.Description
            };

            return metadata;
        }

        public async Task<List<string>> OptimizeKeywordsAsync(string content, List<string> targetKeywords)
        {
            var suggestedKeywords = await _seoService.SuggestKeywordsAsync(content, 10);
            var optimizedKeywords = new List<string>();

            // Combine target and suggested keywords
            optimizedKeywords.AddRange(targetKeywords ?? new List<string>());
            optimizedKeywords.AddRange(suggestedKeywords.Where(k => !optimizedKeywords.Contains(k)));

            return optimizedKeywords.Distinct().Take(10).ToList();
        }

        public async Task<int> GetContentQualityScoreAsync(string content, string title = null)
        {
            var analysis = await _seoService.AnalyzeContentAsync(content, title);
            var readability = await _seoService.CalculateReadabilityScoreAsync(content);

            // Weighted score: SEO (60%) + Readability (40%)
            return (int)(analysis.Score * 0.6 + readability * 0.4);
        }

        private string GenerateCanonicalUrl(string title)
        {
            // This would typically use a slug generator service
            var slug = title.ToLower()
                           .Replace(" ", "-")
                           .Replace(".", "")
                           .Replace(",", "")
                           .Replace("?", "")
                           .Replace("!", "");

            return $"/blog/{slug}";
        }
    }
}