namespace BloggingAgent.Configuration
{
    public class SeoSettings
    {
        public int MinTitleLength { get; set; } = 30;
        public int MaxTitleLength { get; set; } = 60;
        public int MinDescriptionLength { get; set; } = 120;
        public int MaxDescriptionLength { get; set; } = 160;
        public int MinContentLength { get; set; } = 300;
        public double TargetKeywordDensity { get; set; } = 1.5;
        public bool AutoGenerateMetaDescription { get; set; } = true;
        public bool AutoOptimizeContent { get; set; } = true;
        public string[] DefaultKeywords { get; set; } = new[] { "blog", "article", "content" };
    }
}