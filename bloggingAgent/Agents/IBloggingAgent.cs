using System.Collections.Generic;
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
        Task<BlogPostDto> PublishPostAsync(BlogPostDto post);
        Task<BlogPostDto> UpdatePostAsync(Guid postId, BlogPostDto updatedPost);
        Task<bool> DeletePostAsync(Guid postId);
        Task<List<BlogPostDto>> GetRelatedPostsAsync(Guid postId, int count = 5);
        Task<Dictionary<string, int>> GetContentAnalyticsAsync();
    }
}