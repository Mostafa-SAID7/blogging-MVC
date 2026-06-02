using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BloggingAgent.Models.Enums;

namespace BloggingAgent.Models.Domain
{
    public class BlogPost : BaseEntity
    {

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string Excerpt { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        public PostStatus Status { get; set; } = PostStatus.Draft;

        public BlogCategory Category { get; set; } = BlogCategory.Other;

        public ContentTone Tone { get; set; } = ContentTone.Professional;

        public List<string> Tags { get; set; } = new List<string>();

        public int ReadingTimeMinutes { get; set; }

        public int WordCount { get; set; }

        public int Likes { get; set; } = 0;
        public int Shares { get; set; } = 0;

        // Navigation properties
        public virtual ApplicationUser AuthorUser { get; set; }
        public string? AuthorUserId { get; set; }
        public string? AuthorId { get; set; }

        public virtual SeoMetadata SeoMetadata { get; set; }
        public virtual ContentAnalytics Analytics { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Domain methods
        public bool IsPublished
        {
            get => Status == PostStatus.Published;
            set
            {
                if (value)
                    Status = PostStatus.Published;
                else if (Status == PostStatus.Published)
                    Status = PostStatus.Draft;
            }
        }

        public bool IsDraft => Status == PostStatus.Draft;

        public bool IsArchived => Status == PostStatus.Archived;

        public void Publish()
        {
            Status = PostStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            Status = PostStatus.Draft;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            Status = PostStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContent(string newContent, string newExcerpt = null)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("Content cannot be empty", nameof(newContent));

            Content = newContent;
            if (!string.IsNullOrWhiteSpace(newExcerpt))
                Excerpt = newExcerpt;

            WordCount = CalculateWordCount();
            ReadingTimeMinutes = CalculateReadingTime();
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag cannot be empty", nameof(tag));

            if (!Tags.Contains(tag))
            {
                Tags.Add(tag.ToLowerInvariant());
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void RemoveTag(string tag)
        {
            if (Tags.Remove(tag.ToLowerInvariant()))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }

        private int CalculateWordCount()
        {
            if (string.IsNullOrEmpty(Content)) return 0;
            var words = Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        private int CalculateReadingTime()
        {
            const int wordsPerMinute = 200;
            return Math.Max(1, (int)Math.Ceiling((double)WordCount / wordsPerMinute));
        }

        public void AddDomainEvent(object domainEvent)
        {
            // Domain event stub - events collected here for future CQRS use
        }
    }
}
