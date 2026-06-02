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
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("Prompt is empty or null");
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            var provider = GetAvailableProvider();
            if (provider == null)
            {
                _logger.LogError("No LLM provider is available");
                throw new InvalidOperationException("No LLM provider is available.");
            }

            try
            {
                _logger.LogInformation("Generating content using {Provider} with prompt length: {Length}", provider.ProviderName, prompt.Length);
                var result = await provider.GenerateTextAsync(prompt, maxTokens);
                
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogWarning("LLM returned empty result");
                    return null;
                }

                return result;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "LLM provider {Provider} is not configured properly", provider.ProviderName);
                
                // Try fallback provider
                var fallbackProvider = GetFallbackProvider(provider);
                if (fallbackProvider != null)
                {
                    _logger.LogInformation("Falling back to {FallbackProvider}", fallbackProvider.ProviderName);
                    return await fallbackProvider.GenerateTextAsync(prompt, maxTokens);
                }
                
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "LLM request timed out for provider {Provider}", provider.ProviderName);
                throw new InvalidOperationException($"LLM request timed out. Please try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating content with provider {Provider}", provider.ProviderName);
                throw;
            }
        }

        public async Task<string> GenerateWithContextAsync(string prompt, string context, int maxTokens = 1000)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("Prompt is empty or null");
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            var provider = GetAvailableProvider();
            if (provider == null)
            {
                _logger.LogError("No LLM provider is available");
                throw new InvalidOperationException("No LLM provider is available.");
            }

            try
            {
                _logger.LogInformation("Generating content with context using {Provider}", provider.ProviderName);
                var result = await provider.CompleteTextAsync(prompt, context, maxTokens);
                
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogWarning("LLM returned empty result");
                    return null;
                }

                return result;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "LLM provider {Provider} is not configured properly", provider.ProviderName);
                
                // Try fallback provider
                var fallbackProvider = GetFallbackProvider(provider);
                if (fallbackProvider != null)
                {
                    _logger.LogInformation("Falling back to {FallbackProvider}", fallbackProvider.ProviderName);
                    return await fallbackProvider.CompleteTextAsync(prompt, context, maxTokens);
                }
                
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "LLM request timed out for provider {Provider}", provider.ProviderName);
                throw new InvalidOperationException($"LLM request timed out. Please try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating content with context using provider {Provider}", provider.ProviderName);
                throw;
            }
        }

        private ILlmProvider GetAvailableProvider()
        {
            // Priority: OpenAI > Ollama
            return _providers.FirstOrDefault(p => p.IsAvailable && p.ProviderName == "OpenAI") ??
                   _providers.FirstOrDefault(p => p.IsAvailable);
        }

        private ILlmProvider GetFallbackProvider(ILlmProvider currentProvider)
        {
            // Get first available provider that's different from current
            return _providers.FirstOrDefault(p => p.IsAvailable && p.ProviderName != currentProvider.ProviderName);
        }
    }
}
