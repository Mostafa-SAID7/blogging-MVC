using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class SocialMediaPost : BaseEntity
    {
        // Relationships
        public Guid SocialMediaAccountId { get; set; }
        public virtual SocialMediaAccount Account { get; set; }

        public Guid? BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        // Platform-specific IDs
        public string PlatformPostId { get; set; }
        public string Platform { get; set; }

        // Content
        public string Content { get; set; }
        public string Title { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public string PostUrl { get; set; }

        // Status and scheduling
        public string Status { get; set; } // "draft", "scheduled", "posted", "failed"
        public DateTime? ScheduledAt { get; set; }
        public DateTime? PostedAt { get; set; }

        // Engagement metrics
        public int Likes { get; set; } = 0;
        public int Shares { get; set; } = 0;
        public int Comments { get; set; } = 0;
        public int Views { get; set; } = 0;
        public int Reach { get; set; } = 0;
        public int Impressions { get; set; } = 0;

        // Performance tracking
        public decimal EngagementRate { get; set; } = 0;
        public DateTime? LastSyncedAt { get; set; }

        // Error handling
        public string ErrorMessage { get; set; }
        public int RetryCount { get; set; } = 0;
        public DateTime? LastRetryAt { get; set; }

        // Metadata
        public Dictionary<string, object> PlatformMetadata { get; set; } = new Dictionary<string, object>();
        public string Tags { get; set; } // Comma-separated tags
        public string Campaign { get; set; } // Marketing campaign identifier

        // Computed properties
        public bool IsPosted => Status == "posted";
        public bool IsScheduled => Status == "scheduled";
        public bool HasFailed => Status == "failed";
        public bool CanRetry => HasFailed && RetryCount < 3;
        public TimeSpan? TimeUntilPost => ScheduledAt.HasValue ? ScheduledAt.Value - DateTime.UtcNow : null;
        public bool IsOverdue => ScheduledAt.HasValue && ScheduledAt.Value < DateTime.UtcNow && !IsPosted;
        public string StatusDisplay => GetStatusDisplay();
        public string StatusColor => GetStatusColor();

        // Methods
        public void MarkAsPosted(string platformPostId, DateTime postedAt)
        {
            Status = "posted";
            PlatformPostId = platformPostId;
            PostedAt = postedAt;
            MarkAsModified();
        }

        public void MarkAsFailed(string errorMessage)
        {
            Status = "failed";
            ErrorMessage = errorMessage;
            RetryCount++;
            LastRetryAt = DateTime.UtcNow;
            MarkAsModified();
        }

        public void ResetForRetry()
        {
            Status = "scheduled";
            ErrorMessage = null;
            LastRetryAt = DateTime.UtcNow;
            MarkAsModified();
        }

        public void UpdateEngagement(int likes, int shares, int comments, int views = 0, int reach = 0, int impressions = 0)
        {
            Likes = likes;
            Shares = shares;
            Comments = comments;
            Views = views;
            Reach = reach;
            Impressions = impressions;

            // Calculate engagement rate
            if (Reach > 0)
            {
                EngagementRate = (decimal)(likes + shares + comments) / reach * 100;
            }

            LastSyncedAt = DateTime.UtcNow;
            MarkAsModified();
        }

        public void Schedule(DateTime scheduledAt)
        {
            Status = "scheduled";
            ScheduledAt = scheduledAt;
            MarkAsModified();
        }

        private string GetStatusDisplay()
        {
            return Status switch
            {
                "draft" => "Draft",
                "scheduled" => "Scheduled",
                "posted" => "Posted",
                "failed" => "Failed",
                _ => "Unknown"
            };
        }

        private string GetStatusColor()
        {
            return Status switch
            {
                "draft" => "secondary",
                "scheduled" => "info",
                "posted" => "success",
                "failed" => "danger",
                _ => "secondary"
            };
        }
    }
}