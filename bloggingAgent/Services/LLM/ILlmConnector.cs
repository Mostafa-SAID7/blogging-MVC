using System.Threading.Tasks;

namespace BloggingAgent.Services.LLM
{
    public interface ILlmConnector
    {
        Task<string> GenerateContentAsync(string prompt, int maxTokens = 1000);
        Task<string> GenerateWithContextAsync(string prompt, string context, int maxTokens = 1000);
        bool IsConfigured { get; }
    }
}