using System.Collections.Generic;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Services.SEO
{
    public interface ISeoAnalyzer
    {
        Task<SeoMetadata> GenerateSeoMetadataAsync(string title, string content, List<string> keywords = null);
        Task<List<string>> OptimizeKeywordsAsync(string content, List<string> targetKeywords);
        Task<int> GetContentQualityScoreAsync(string content, string title = null);
    }
}
