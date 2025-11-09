using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.ViewModels
{
    public class SettingsViewModel
    {
        public AgentSettings Settings { get; set; }
        public bool SaveSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}