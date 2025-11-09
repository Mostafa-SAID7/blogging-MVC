namespace BloggingAgent.Configuration
{
    public class OpenAISettings
    {
        public string ApiKey { get; set; }
        public string Model { get; set; } = "gpt-3.5-turbo";
        public string Organization { get; set; }
        public int MaxTokens { get; set; } = 1000;
        public double Temperature { get; set; } = 0.7;
        public int TimeoutSeconds { get; set; } = 30;
    }
}