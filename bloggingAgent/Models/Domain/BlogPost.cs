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
        public string AuthorId { get; set; } // Changed from string Author to string AuthorId
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public SeoMetadata SeoMetadata { get; set; }
        public ContentAnalytics Analytics { get; set; }

        // Navigation properties
        public virtual ApplicationUser Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<BlogPostCategory> PostCategories { get; set; } = new List<BlogPostCategory>();
    }

    // Junction table for many-to-many relationship between BlogPost and Category
    public class BlogPostCategory
    {
        public int BlogPostId { get; set; }
        public int CategoryId { get; set; }

        public virtual BlogPost BlogPost { get; set; }
        public virtual Category Category { get; set; }
    }
}