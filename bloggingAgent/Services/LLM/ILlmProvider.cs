using System.Threading.Tasks;

namespace BloggingAgent.Services.LLM
{
    public interface ILlmProvider
    {
        Task<string> GenerateTextAsync(string prompt, int maxTokens = 1000);
        Task<string> CompleteTextAsync(string prompt, string context, int maxTokens = 1000);
        bool IsAvailable { get; }
        string ProviderName { get; }
    }
}