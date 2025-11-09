using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.LLM
{
    public class OllamaProvider : ILlmProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OllamaProvider> _logger;
        private readonly LlmSettings _settings;

        public OllamaProvider(HttpClient httpClient, ILogger<OllamaProvider> logger, IOptions<LlmSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings.Value;
        }

        public bool IsAvailable => !string.IsNullOrEmpty(_settings.OllamaEndpoint);
        public string ProviderName => "Ollama";

        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 1000)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("Ollama provider is not configured.");

            var requestBody = new
            {
                model = _settings.OllamaModel ?? "llama2",
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_settings.OllamaEndpoint}/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            var generatedText = responseJson.RootElement.GetProperty("response").GetString();

            return generatedText;
        }

        public async Task<string> CompleteTextAsync(string prompt, string context, int maxTokens = 1000)
        {
            var fullPrompt = $"{context}\n\n{prompt}";
            return await GenerateTextAsync(fullPrompt, maxTokens);
        }
    }
}