using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class SeoMetadata : BaseEntity
    {
        public Guid BlogPostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string CanonicalUrl { get; set; }
        public string OgTitle { get; set; }
        public string OgDescription { get; set; }
        public string OgImage { get; set; }
        public string TwitterCard { get; set; }
        public Dictionary<string, string> StructuredData { get; set; } = new Dictionary<string, string>();
        
        // Note: Navigation property removed to prevent shadow key generation.
        // The relationship is fully configured in BlogPostConfiguration.
    }
}