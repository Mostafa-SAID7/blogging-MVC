using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public SeoMetadata SeoMetadata { get; set; }
        public ContentAnalytics Analytics { get; set; }
    }
}