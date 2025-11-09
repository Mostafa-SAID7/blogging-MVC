using System;
using System.Collections.Generic;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.DTOs
{
    public class HomeViewModel
    {
        public List<BlogPostDto> FeaturedPosts { get; set; } = new List<BlogPostDto>();
        public PlatformStats PlatformStats { get; set; }
        public List<RecentActivity> RecentActivity { get; set; } = new List<RecentActivity>();
        public List<CategoryInfo> PopularCategories { get; set; } = new List<CategoryInfo>();
        public bool IsAuthenticated { get; set; }
    }

    public class PlatformStats
    {
        public int TotalPosts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalComments { get; set; }
        public int TotalViews { get; set; }
    }

    public class RecentActivity
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CategoryInfo
    {
        public string Name { get; set; }
        public int PostCount { get; set; }
        public string Slug { get; set; }
    }

    public class AboutViewModel
    {
        public string PlatformName { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; } = new List<string>();
        public List<string> Technologies { get; set; } = new List<string>();
    }

    public class FeaturesViewModel
    {
        public List<FeatureCategory> FeatureCategories { get; set; } = new List<FeatureCategory>();
    }

    public class FeatureCategory
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; } = new List<string>();
    }

    public class PricingViewModel
    {
        public List<PricingPlan> Plans { get; set; } = new List<PricingPlan>();
        public List<FAQ> FAQs { get; set; } = new List<FAQ>();
    }

    public class PricingPlan
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Period { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; } = new List<string>();
        public string ButtonText { get; set; }
        public string ButtonClass { get; set; }
        public bool IsPopular { get; set; }
    }

    public class FAQ
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class ContactViewModel
    {
        public ContactInfo ContactInfo { get; set; }
        public List<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    }

    public class ContactInfo
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BusinessHours { get; set; }
    }

    public class SocialLink
    {
        public string Platform { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}