using System;
using System.Threading.Tasks;

namespace BloggingAgent.Services.Cache
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<bool> ExistsAsync(string key);
        Task RemoveAsync(string key);
        Task ClearAsync();
    }
}