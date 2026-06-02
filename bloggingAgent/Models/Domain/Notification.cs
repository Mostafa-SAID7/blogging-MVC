using System;

namespace BloggingAgent.Models.Domain
{
    public class Notification : BaseEntity
    {
        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Notification content
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // "comment", "like", "follow", "system", "marketing"

        // Status
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // Related entity (optional)
        public string RelatedEntityType { get; set; } // "BlogPost", "Comment", "User", etc.
        public string RelatedEntityId { get; set; }

        // Action URL (optional)
        public string ActionUrl { get; set; }

        // Priority
        public string Priority { get; set; } = "normal"; // "low", "normal", "high", "urgent"

        // Metadata
        public DateTime? ExpiresAt { get; set; }

        // Computed properties
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
        public string TimeAgo => GetTimeAgo();
        public string PriorityColor => GetPriorityColor();
        public string TypeIcon => GetTypeIcon();
        public string TypeColor => GetTypeColor();
        public bool CanAction => !string.IsNullOrEmpty(ActionUrl);

        // Methods
        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }

        public void MarkAsUnread()
        {
            IsRead = false;
            ReadAt = null;
        }

        private string GetTimeAgo()
        {
            var timeSpan = DateTime.UtcNow - CreatedAt;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";

            return CreatedAt.ToString("MMM dd, yyyy");
        }

        private string GetPriorityColor()
        {
            return Priority.ToLower() switch
            {
                "urgent" => "danger",
                "high" => "warning",
                "low" => "info",
                _ => "secondary"
            };
        }

        private string GetTypeIcon()
        {
            return Type.ToLower() switch
            {
                "comment" => "fas fa-comment",
                "like" => "fas fa-heart",
                "follow" => "fas fa-user-plus",
                "system" => "fas fa-cog",
                "marketing" => "fas fa-bullhorn",
                _ => "fas fa-bell"
            };
        }

        private string GetTypeColor()
        {
            return Type.ToLower() switch
            {
                "comment" => "info",
                "like" => "danger",
                "follow" => "success",
                "system" => "warning",
                "marketing" => "primary",
                _ => "secondary"
            };
        }
    }
}