using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.Memory
{
    public class MemoryAnalyzer : IMemoryAnalyzer
    {
        private readonly IMemoryService _memoryService;
        private readonly ILogger<MemoryAnalyzer> _logger;

        public MemoryAnalyzer(IMemoryService memoryService, ILogger<MemoryAnalyzer> logger)
        {
            _memoryService = memoryService;
            _logger = logger;
        }

        public async Task<Dictionary<string, int>> GetCategoryCountsAsync()
        {
            var allMemories = await _memoryService.GetByCategoryAsync(null); // Get all categories
            return allMemories.GroupBy(m => m.Category)
                             .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<List<string>> GetRecentMemoriesAsync(int count = 10)
        {
            var allMemories = await _memoryService.GetByCategoryAsync(null);
            return allMemories.OrderByDescending(m => m.CreatedAt)
                             .Take(count)
                             .Select(m => $"{m.Key}: {m.Value}")
                             .ToList();
        }

        public async Task<bool> HasMemoryForTopicAsync(string topic)
        {
            var key = $"topic_{topic.ToLower()}";
            return await _memoryService.ExistsAsync(key);
        }

        public async Task StoreTopicMemoryAsync(string topic, string content)
        {
            var key = $"topic_{topic.ToLower()}";
            await _memoryService.StoreAsync(key, content, "topics");
        }
    }
}