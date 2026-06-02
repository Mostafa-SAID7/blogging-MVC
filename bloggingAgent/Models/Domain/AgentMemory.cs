using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class AgentMemory : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Category { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}