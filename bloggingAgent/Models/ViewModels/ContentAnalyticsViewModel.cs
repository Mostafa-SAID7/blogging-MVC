namespace BloggingAgent.Models.ViewModels
{
    public class ContentAnalyticsViewModel
    {
        public Guid BlogPostId { get; set; }
        public int Views { get; set; }
        public int UniqueViews { get; set; }
        public int Shares { get; set; }
        public int Comments { get; set; }
        public double AverageReadTime { get; set; }
        public double BounceRate { get; set; }
    }
}
