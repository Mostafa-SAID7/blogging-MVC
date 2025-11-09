using System.Threading.Tasks;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Services.SEO
{
    public interface ISeoService
    {
        Task<SeoAnalysisResult> AnalyzeContentAsync(string content, string title = null);
        Task<string> GenerateMetaDescriptionAsync(string content);
        Task<string[]> SuggestKeywordsAsync(string content, int count = 5);
        Task<int> CalculateReadabilityScoreAsync(string content);
    }
}