using System.Collections.Generic;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.ViewModels
{
    public class AnalyticsViewModel
    {
        public List<ContentAnalytics> PostAnalytics { get; set; } = new List<ContentAnalytics>();
        public int TotalViews { get; set; }
        public int TotalPosts { get; set; }
        public double AverageReadTime { get; set; }
        public Dictionary<string, int> TopTags { get; set; } = new Dictionary<string, int>();
        public List<KeyValuePair<string, int>> TrafficSources { get; set; } = new List<KeyValuePair<string, int>>();
        public Dictionary<string, double> PerformanceMetrics { get; set; } = new Dictionary<string, double>();
    }
}