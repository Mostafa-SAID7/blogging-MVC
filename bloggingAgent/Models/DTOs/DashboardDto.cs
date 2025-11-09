using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.DTOs
{
    public class DashboardViewModel
    {
        public UserStatsDto UserStats { get; set; }
        public List<BlogPostDto> RecentPosts { get; set; } = new List<BlogPostDto>();
        public List<CommentDto> RecentComments { get; set; } = new List<CommentDto>();
        public Dictionary<string, int> ContentAnalytics { get; set; } = new Dictionary<string, int>();
        public List<NotificationDto> RecentNotifications { get; set; } = new List<NotificationDto>();
        public List<QuickAction> QuickActions { get; set; } = new List<QuickAction>();
        public List<UpcomingTask> UpcomingTasks { get; set; } = new List<UpcomingTask>();
    }

    public class QuickAction
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
    }

    public class UpcomingTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public string ActionUrl { get; set; }
    }

    public class DashboardAnalyticsViewModel
    {
        public Dictionary<string, object> OverviewStats { get; set; } = new Dictionary<string, object>();
        public List<Dictionary<string, object>> PostPerformance { get; set; } = new List<Dictionary<string, object>>();
        public Dictionary<string, object> AudienceInsights { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ContentTrends { get; set; } = new Dictionary<string, object>();
        public string TimeRange { get; set; }
    }

    public class ContentManagementViewModel
    {
        public List<BlogPostDto> AllPosts { get; set; } = new List<BlogPostDto>();
        public int DraftCount { get; set; }
        public int PublishedCount { get; set; }
        public int TotalViews { get; set; }
        public int TotalComments { get; set; }
    }
}