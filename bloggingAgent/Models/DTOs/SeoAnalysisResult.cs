using System.Collections.Generic;

namespace BloggingAgent.Models.DTOs
{
    public class SeoAnalysisResult
    {
        public int Score { get; set; }
        public List<string> Suggestions { get; set; } = new List<string>();
        public Dictionary<string, bool> Checks { get; set; } = new Dictionary<string, bool>();
        public string KeywordDensity { get; set; }
        public List<string> MissingElements { get; set; } = new List<string>();
        public Dictionary<string, int> KeywordOccurrences { get; set; } = new Dictionary<string, int>();
    }
}