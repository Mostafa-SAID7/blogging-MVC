using System.Collections.Generic;

namespace BloggingAgent.Models.DTOs
{
    public class GeneratePostRequest
    {
        public string Topic { get; set; }
        public string Keywords { get; set; }
        public int TargetWordCount { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Tone { get; set; }
        public string TargetAudience { get; set; }
        public bool IncludeImages { get; set; }
        public Dictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();
    }
}