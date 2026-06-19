using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class ContentAnalytics : BaseEntity
    {
        public Guid BlogPostId { get; set; }
        // Note: Navigation property removed to prevent shadow key generation.
        // The relationship is fully configured in BlogPostConfiguration.
        public int Views { get; set; }
        public int UniqueViews { get; set; }
        public int Shares { get; set; }
        public int Comments { get; set; }
        public double AverageReadTime { get; set; }
        public double BounceRate { get; set; }
        public Dictionary<string, int> TrafficSources { get; set; } = new Dictionary<string, int>();
    }
}