namespace BloggingAgent.Settings
{
    public class CacheSettings
    {
        public int DefaultExpirationMinutes { get; set; } = 30;
        public int BlogPostExpirationMinutes { get; set; } = 10;
        public int AnalyticsExpirationMinutes { get; set; } = 15;
        public int MemoryExpirationDays { get; set; } = 30;
        public bool EnableCaching { get; set; } = true;
        public int MaxCacheSize { get; set; } = 1000;
        public string CacheType { get; set; } = "Memory"; // Memory, Redis, Distributed
    }
}
