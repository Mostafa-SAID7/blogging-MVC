using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class SocialMediaAccount : BaseEntity
    {
        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Platform information
        public string Platform { get; set; } // "twitter", "linkedin", "facebook", "instagram", "youtube", "tiktok"
        public string PlatformUserId { get; set; }
        public string PlatformUsername { get; set; }
        public string PlatformDisplayName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }

        // Account details
        public string ProfileUrl { get; set; }
        public string AvatarUrl { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public string Bio { get; set; }

        // Settings
        public bool IsActive { get; set; } = true;
        public bool AutoPostEnabled { get; set; } = false;
        public string DefaultPostTemplate { get; set; }
        public Dictionary<string, object> PlatformSettings { get; set; } = new Dictionary<string, object>();

        // Statistics
        public int TotalPosts { get; set; } = 0;
        public int TotalEngagement { get; set; } = 0;
        public DateTime? LastPostAt { get; set; }
        public DateTime? LastSyncAt { get; set; }

        // Computed properties
        public bool IsTokenExpired => TokenExpiresAt.HasValue && TokenExpiresAt.Value < DateTime.UtcNow;
        public bool NeedsRefresh => IsTokenExpired && !string.IsNullOrEmpty(RefreshToken);
        public string PlatformDisplayNameFormatted => GetPlatformDisplayName();
        public string PlatformColor => GetPlatformColor();
        public string PlatformIcon => GetPlatformIcon();

        // Methods
        public void UpdateToken(string accessToken, DateTime? expiresAt = null, string refreshToken = null)
        {
            AccessToken = accessToken;
            TokenExpiresAt = expiresAt;
            if (!string.IsNullOrEmpty(refreshToken))
            {
                RefreshToken = refreshToken;
            }
            MarkAsModified();
        }

        public void IncrementPostCount()
        {
            TotalPosts++;
            LastPostAt = DateTime.UtcNow;
            MarkAsModified();
        }

        public void UpdateStats(int followersCount, int followingCount, string bio = null)
        {
            FollowersCount = followersCount;
            FollowingCount = followingCount;
            if (!string.IsNullOrEmpty(bio))
            {
                Bio = bio;
            }
            LastSyncAt = DateTime.UtcNow;
            MarkAsModified();
        }

        public void AddEngagement(int engagementCount)
        {
            TotalEngagement += engagementCount;
            MarkAsModified();
        }

        private string GetPlatformDisplayName()
        {
            return Platform.ToLower() switch
            {
                "twitter" => "Twitter",
                "linkedin" => "LinkedIn",
                "facebook" => "Facebook",
                "instagram" => "Instagram",
                "youtube" => "YouTube",
                "tiktok" => "TikTok",
                "pinterest" => "Pinterest",
                "reddit" => "Reddit",
                _ => Platform
            };
        }

        private string GetPlatformColor()
        {
            return Platform.ToLower() switch
            {
                "twitter" => "#1DA1F2",
                "linkedin" => "#0077B5",
                "facebook" => "#1877F2",
                "instagram" => "#E4405F",
                "youtube" => "#FF0000",
                "tiktok" => "#000000",
                "pinterest" => "#E60023",
                "reddit" => "#FF4500",
                _ => "#6c757d"
            };
        }

        private string GetPlatformIcon()
        {
            return Platform.ToLower() switch
            {
                "twitter" => "fab fa-twitter",
                "linkedin" => "fab fa-linkedin",
                "facebook" => "fab fa-facebook",
                "instagram" => "fab fa-instagram",
                "youtube" => "fab fa-youtube",
                "tiktok" => "fab fa-tiktok",
                "pinterest" => "fab fa-pinterest",
                "reddit" => "fab fa-reddit",
                _ => "fas fa-share-alt"
            };
        }
    }
}