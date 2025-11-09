using System;
using System.Collections.Generic;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.DTOs
{
    public class SocialPlatform
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class SocialPostsViewModel
    {
        public List<SocialMediaPost> Posts { get; set; } = new List<SocialMediaPost>();
        public List<SocialMediaAccount> Accounts { get; set; } = new List<SocialMediaAccount>();
        public int SelectedAccountId { get; set; }
    }

    public class CreateSocialPostViewModel
    {
        public List<SocialMediaAccount> AvailableAccounts { get; set; } = new List<SocialMediaAccount>();
        public List<int> SelectedAccountIds { get; set; } = new List<int>();
        public int? BlogPostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Campaign { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }

    public class SocialAnalyticsViewModel
    {
        public List<SocialMediaAccount> Accounts { get; set; } = new List<SocialMediaAccount>();
        public int TotalPosts { get; set; }
        public int PostedPosts { get; set; }
        public int ScheduledPosts { get; set; }
        public int FailedPosts { get; set; }
        public int TotalEngagement { get; set; }
        public decimal AverageEngagementRate { get; set; }
        public Dictionary<string, int> PostsByPlatform { get; set; } = new Dictionary<string, int>();
        public List<SocialMediaPost> RecentPosts { get; set; } = new List<SocialMediaPost>();
    }
}