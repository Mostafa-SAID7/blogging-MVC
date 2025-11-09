using System.Threading.Tasks;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Agents
{
    public interface IBloggingAgent
    {
        Task<BlogPostDto> GeneratePostAsync(GeneratePostRequest request);
        Task<SeoAnalysisResult> AnalyzePostAsync(string content, string title = null);
        Task<string> OptimizeContentAsync(string content, string[] keywords);
        Task<string> GenerateExcerptAsync(string content);
        Task<string[]> SuggestTagsAsync(string content);
        Task<bool> IsTopicRelevantAsync(string topic);
    }
}