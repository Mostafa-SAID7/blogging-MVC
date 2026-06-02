using System;

namespace BloggingAgent.Models.Domain
{
    public class Subscription : BaseEntity
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Subscription Details
        public string PlanName { get; set; } // "Free", "Pro", "Enterprise"
        public string StripeSubscriptionId { get; set; }
        public string StripeCustomerId { get; set; }
        public string Status { get; set; } // "active", "canceled", "past_due", "unpaid"

        // Billing Information
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string BillingInterval { get; set; } // "month", "year"
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? EndedAt { get; set; }

        // Plan Limits and Features
        public int MaxPostsPerMonth { get; set; }
        public int MaxAiTokensPerMonth { get; set; }
        public bool HasAdvancedAnalytics { get; set; }
        public bool HasPrioritySupport { get; set; }
        public bool HasWhiteLabel { get; set; }
        public bool HasApiAccess { get; set; }
        public int MaxTeamMembers { get; set; }

        // Usage Tracking
        public int PostsUsedThisMonth { get; set; } = 0;
        public int AiTokensUsedThisMonth { get; set; } = 0;
        public DateTime UsageResetDate { get; set; }

        // Computed Properties
        public bool IsActive => Status == "active" && CurrentPeriodEnd > DateTime.UtcNow;
        public bool IsTrial => false; // Could be extended for trial periods
        public int DaysUntilRenewal => (CurrentPeriodEnd - DateTime.UtcNow).Days;
        public double UsagePercentage => MaxPostsPerMonth > 0 ? (double)PostsUsedThisMonth / MaxPostsPerMonth * 100 : 0;

        // Methods
        public bool CanCreatePost()
        {
            if (PlanName == "Free" && PostsUsedThisMonth >= MaxPostsPerMonth)
                return false;
            return true;
        }

        public bool CanUseAiTokens(int tokens)
        {
            if (PlanName == "Free" && (AiTokensUsedThisMonth + tokens) > MaxAiTokensPerMonth)
                return false;
            return true;
        }

        public void ResetMonthlyUsage()
        {
            PostsUsedThisMonth = 0;
            AiTokensUsedThisMonth = 0;
            UsageResetDate = DateTime.UtcNow.AddMonths(1);
            MarkAsModified();
        }

        public void IncrementPostUsage()
        {
            PostsUsedThisMonth++;
            MarkAsModified();
        }

        public void IncrementAiUsage(int tokens)
        {
            AiTokensUsedThisMonth += tokens;
            MarkAsModified();
        }
    }
}