using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly ConcurrentDictionary<string, DateTime> _expirationTimes;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _expirationTimes = new ConcurrentDictionary<string, DateTime>();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return value;
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
                _expirationTimes[key] = DateTime.UtcNow.Add(expiration.Value);
            }
            else
            {
                // Default 30 minutes
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                _expirationTimes[key] = DateTime.UtcNow.AddMinutes(30);
            }

            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _expirationTimes.TryRemove(key.ToString(), out _);
                _logger.LogDebug("Cache entry evicted: {Key}, Reason: {Reason}", key, reason);
            });

            _cache.Set(key, value, options);
            _logger.LogDebug("Cached value for key: {Key}", key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _expirationTimes.TryRemove(key, out _);
            _logger.LogDebug("Removed cache entry: {Key}", key);
        }

        public async Task ClearAsync()
        {
            // Note: IMemoryCache doesn't have a clear method, so we need to be creative
            // In a real implementation, you might use a distributed cache or maintain a list of keys
            _logger.LogWarning("ClearAsync not fully implemented for MemoryCache - consider using distributed cache");

            // For now, we'll just log that this operation is not supported
            // In production, you'd want to use IDistributedCache or maintain your own key registry
        }

        public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            if (_expirationTimes.TryGetValue(key, out DateTime expirationTime))
            {
                var timeToLive = expirationTime - DateTime.UtcNow;
                return timeToLive > TimeSpan.Zero ? timeToLive : null;
            }

            return null;
        }
    }
}