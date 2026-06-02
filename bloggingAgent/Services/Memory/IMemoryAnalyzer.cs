using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloggingAgent.Services.Memory
{
    public interface IMemoryAnalyzer
    {
        Task<Dictionary<string, int>> GetCategoryCountsAsync();
        Task<System.Collections.Generic.List<string>> GetRecentMemoriesAsync(int count = 10);
        Task<bool> HasMemoryForTopicAsync(string topic);
        Task StoreTopicMemoryAsync(string topic, string content);
    }
}
