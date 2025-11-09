using System.Threading.Tasks;

namespace BloggingAgent.Services.Content
{
    public interface IContentFormatter
    {
        Task<string> FormatAsHtmlAsync(string markdownContent);
        Task<string> FormatAsMarkdownAsync(string htmlContent);
        Task<string> ExtractExcerptAsync(string content, int maxLength = 150);
        Task<string> AddImagePlaceholdersAsync(string content);
        Task<string> OptimizeForWebAsync(string content);
    }
}