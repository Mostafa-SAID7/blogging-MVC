using System;

namespace BloggingAgent.Models.Domain
{
    public class UserFollow : BaseEntity
    {
        // The user who is following
        public string FollowerId { get; set; }
        public virtual ApplicationUser Follower { get; set; }

        // The user being followed
        public string FollowingId { get; set; }
        public virtual ApplicationUser Following { get; set; }

        // Optional: Follow request status for private accounts
        public bool IsAccepted { get; set; } = true; // Default to true for public follows
        public DateTime? AcceptedAt { get; set; }

        // For notifications and analytics
        public bool NotificationsEnabled { get; set; } = true;
    }
}