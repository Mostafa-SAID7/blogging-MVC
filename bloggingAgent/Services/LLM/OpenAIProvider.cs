using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.LLM
{
    public class OpenAIProvider : ILlmProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenAIProvider> _logger;
        private readonly OpenAISettings _settings;

        public OpenAIProvider(HttpClient httpClient, ILogger<OpenAIProvider> logger, IOptions<OpenAISettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings.Value;
        }

        public bool IsAvailable => !string.IsNullOrEmpty(_settings.ApiKey);
        public string ProviderName => "OpenAI";

        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 1000)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("OpenAI provider is not configured.");

            var requestBody = new
            {
                model = _settings.Model ?? "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = maxTokens,
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            var generatedText = responseJson.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return generatedText;
        }

        public async Task<string> CompleteTextAsync(string prompt, string context, int maxTokens = 1000)
        {
            var fullPrompt = $"{context}\n\n{prompt}";
            return await GenerateTextAsync(fullPrompt, maxTokens);
        }
    }
}