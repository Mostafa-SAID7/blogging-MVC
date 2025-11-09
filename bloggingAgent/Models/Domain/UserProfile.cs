using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Profile Information
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string CoverImageUrl { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Social Links
        public string TwitterHandle { get; set; }
        public string LinkedInProfile { get; set; }
        public string GitHubProfile { get; set; }
        public string InstagramHandle { get; set; }
        public string YouTubeChannel { get; set; }

        // Professional Information
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> Interests { get; set; } = new List<string>();

        // Blogging Preferences
        public List<string> FavoriteCategories { get; set; } = new List<string>();
        public string PreferredTone { get; set; } = "professional";
        public string WritingStyle { get; set; }
        public List<string> PreferredKeywords { get; set; } = new List<string>();

        // Statistics
        public int TotalPosts { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
        public int TotalLikes { get; set; } = 0;
        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
        public double AveragePostRating { get; set; } = 0.0;

        // Privacy Settings
        public bool IsProfilePublic { get; set; } = true;
        public bool ShowEmail { get; set; } = false;
        public bool ShowStats { get; set; } = true;
        public bool AllowMessages { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;

        // Activity Tracking
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActiveAt { get; set; }
        public DateTime? LastPostAt { get; set; }

        // Computed Properties
        public string FullLocation => $"{Location}";
        public string ProfessionalSummary => $"{JobTitle} at {Company}".Trim();
        public bool IsActive => LastActiveAt.HasValue &&
                               (DateTime.UtcNow - LastActiveAt.Value).TotalDays <= 30;
    }
}