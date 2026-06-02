namespace BloggingAgent.Settings
{
    public class ContentSettings
    {
        public int DefaultExcerptLength { get; set; } = 150;
        public string DefaultImagePath { get; set; } = "/images/placeholder.jpg";
        public string CriticalCssPath { get; set; }
        public string CriticalJsPath { get; set; }
        public bool EnableImageOptimization { get; set; } = true;
        public bool EnableLazyLoading { get; set; } = true;
        public bool EnableSchemaMarkup { get; set; } = true;
        public bool EnableAutoToc { get; set; } = false;
        public int MinTocHeadings { get; set; } = 3;
        public string[] AllowedImageDomains { get; set; } = new[] { "images.unsplash.com", "via.placeholder.com" };
        public int MaxImageWidth { get; set; } = 1200;
        public int MaxImageHeight { get; set; } = 800;
        public string ImageQuality { get; set; } = "high";
        public bool EnableResponsiveImages { get; set; } = true;
        public string[] SupportedImageFormats { get; set; } = new[] { "jpg", "jpeg", "png", "webp", "gif" };
        public bool EnableAccessibility { get; set; } = true;
        public bool EnableShortcodes { get; set; } = true;
        public int WordsPerMinute { get; set; } = 200;
        public bool EnableReadingTime { get; set; } = true;
        public string[] EnabledShortcodes { get; set; } = new[] { "youtube", "tweet", "codepen", "gist" };
    }
}
