using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.DTOs
{
    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
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
        public string PreferredTone { get; set; }
        public string WritingStyle { get; set; }
        public List<string> PreferredKeywords { get; set; } = new List<string>();

        // Statistics
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public double AveragePostRating { get; set; }

        // Privacy Settings
        public bool IsProfilePublic { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowStats { get; set; }
        public bool AllowMessages { get; set; }
        public bool EmailNotifications { get; set; }

        // Activity
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public DateTime? LastPostAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsFollowing { get; set; } // For current user context
        public bool CanFollow { get; set; } // For current user context
    }

    public class UpdateProfileRequest
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
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
        public string PreferredTone { get; set; }
        public string WritingStyle { get; set; }
        public List<string> PreferredKeywords { get; set; } = new List<string>();

        // Privacy Settings
        public bool IsProfilePublic { get; set; } = true;
        public bool ShowEmail { get; set; } = false;
        public bool ShowStats { get; set; } = true;
        public bool AllowMessages { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;
    }

    public class UserStatsDto
    {
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public double AveragePostRating { get; set; }
        public Dictionary<string, int> PostsByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TopCategories { get; set; } = new Dictionary<string, int>();
        public List<string> RecentActivity { get; set; } = new List<string>();
    }
}