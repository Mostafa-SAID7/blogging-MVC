using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.LLM
{
    public class LlmConnector : ILlmConnector
    {
        private readonly IEnumerable<ILlmProvider> _providers;
        private readonly ILogger<LlmConnector> _logger;
        private readonly LlmSettings _settings;

        public LlmConnector(IEnumerable<ILlmProvider> providers, ILogger<LlmConnector> logger, IOptions<LlmSettings> settings)
        {
            _providers = providers;
            _logger = logger;
            _settings = settings.Value;
        }

        public bool IsConfigured => _providers.Any(p => p.IsAvailable);

        public async Task<string> GenerateContentAsync(string prompt, int maxTokens = 1000)
        {
            var provider = GetAvailableProvider();
            if (provider == null)
                throw new InvalidOperationException("No LLM provider is available.");

            _logger.LogInformation("Generating content using {Provider}", provider.ProviderName);
            return await provider.GenerateTextAsync(prompt, maxTokens);
        }

        public async Task<string> GenerateWithContextAsync(string prompt, string context, int maxTokens = 1000)
        {
            var provider = GetAvailableProvider();
            if (provider == null)
                throw new InvalidOperationException("No LLM provider is available.");

            _logger.LogInformation("Generating content with context using {Provider}", provider.ProviderName);
            return await provider.CompleteTextAsync(prompt, context, maxTokens);
        }

        private ILlmProvider GetAvailableProvider()
        {
            // Priority: OpenAI > Ollama
            return _providers.FirstOrDefault(p => p.IsAvailable && p.ProviderName == "OpenAI") ??
                   _providers.FirstOrDefault(p => p.IsAvailable);
        }
    }
}