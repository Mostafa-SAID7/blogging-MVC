using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.Memory
{
    public class MemoryService : IMemoryService
    {
        private readonly IRepository<AgentMemory> _memoryRepository;
        private readonly ILogger<MemoryService> _logger;

        public MemoryService(IRepository<AgentMemory> memoryRepository, ILogger<MemoryService> logger)
        {
            _memoryRepository = memoryRepository;
            _logger = logger;
        }

        public async Task StoreAsync(string key, string value, string category = null)
        {
            var memory = new AgentMemory
            {
                Key = key,
                Value = value,
                Category = category ?? "general",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Default 30 days
            };

            await _memoryRepository.AddAsync(memory);
            _logger.LogInformation("Stored memory with key: {Key}", key);
        }

        public async Task<string> RetrieveAsync(string key)
        {
            var memory = await _memoryRepository.GetByIdAsync(key);
            if (memory == null || memory.ExpiresAt < DateTime.UtcNow)
                return null;

            return memory.Value;
        }

        public async Task<IEnumerable<AgentMemory>> GetByCategoryAsync(string category)
        {
            var allMemories = await _memoryRepository.GetAllAsync();
            return allMemories.Where(m => m.Category == category && m.ExpiresAt > DateTime.UtcNow);
        }

        public async Task ClearExpiredAsync()
        {
            var allMemories = await _memoryRepository.GetAllAsync();
            var expiredMemories = allMemories.Where(m => m.ExpiresAt < DateTime.UtcNow);

            foreach (var memory in expiredMemories)
            {
                await _memoryRepository.DeleteAsync(memory.Id);
            }

            _logger.LogInformation("Cleared {Count} expired memories", expiredMemories.Count());
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var memory = await _memoryRepository.GetByIdAsync(key);
            return memory != null && memory.ExpiresAt > DateTime.UtcNow;
        }
    }
}