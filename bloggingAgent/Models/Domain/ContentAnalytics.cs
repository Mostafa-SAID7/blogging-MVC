using System;

namespace BloggingAgent.Models.Domain
{
    public class ContentAnalytics
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public int Views { get; set; }
        public int UniqueViews { get; set; }
        public int Shares { get; set; }
        public int Comments { get; set; }
        public double AverageReadTime { get; set; }
        public double BounceRate { get; set; }
        public DateTime LastUpdated { get; set; }
        public Dictionary<string, int> TrafficSources { get; set; } = new Dictionary<string, int>();
    }
}