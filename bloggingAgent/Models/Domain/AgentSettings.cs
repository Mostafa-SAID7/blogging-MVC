using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class AgentSettings
    {
        public string DefaultAuthor { get; set; }
        public int MaxPostLength { get; set; }
        public List<string> DefaultTags { get; set; } = new List<string>();
        public bool AutoPublish { get; set; }
        public string Theme { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }
}