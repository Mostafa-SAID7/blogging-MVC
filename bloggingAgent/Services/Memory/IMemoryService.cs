using System.Collections.Generic;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Services.Memory
{
    public interface IMemoryService
    {
        Task StoreAsync(string key, string value, string category = null);
        Task<string> RetrieveAsync(string key);
        Task<IEnumerable<AgentMemory>> GetByCategoryAsync(string category);
        Task ClearExpiredAsync();
        Task<bool> ExistsAsync(string key);
    }
}