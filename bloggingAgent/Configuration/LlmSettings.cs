namespace BloggingAgent.Configuration
{
    public class LlmSettings
    {
        public string DefaultProvider { get; set; } = "OpenAI";
        public string OllamaEndpoint { get; set; }
        public string OllamaModel { get; set; } = "llama2";
        public int MaxTokens { get; set; } = 1000;
        public double Temperature { get; set; } = 0.7;
        public int TimeoutSeconds { get; set; } = 30;
    }
}