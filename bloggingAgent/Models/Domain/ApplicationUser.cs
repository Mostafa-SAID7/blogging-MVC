using Microsoft.AspNetCore.Identity;

namespace BloggingAgent.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        // Basic Profile Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName => $"{FirstName} {LastName}".Trim();
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }

        // Social Media Links
        public string TwitterHandle { get; set; }
        public string LinkedInProfile { get; set; }
        public string GitHubProfile { get; set; }

        // Account Status
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? EmailConfirmedAt { get; set; }

        // Preferences
        public string PreferredTheme { get; set; } = "light";
        public string TimeZone { get; set; } = "UTC";
        public bool ReceiveEmailNotifications { get; set; } = true;
        public bool ReceiveCommentNotifications { get; set; } = true;

        // Author Statistics
        public int TotalPosts { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
        public int ReputationPoints { get; set; } = 0;

        // Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Methods
        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            EmailConfirmedAt = DateTime.UtcNow;
        }

        public void IncrementPostCount()
        {
            TotalPosts++;
        }

        public void IncrementCommentCount()
        {
            TotalComments++;
        }

        public void AddReputationPoints(int points)
        {
            ReputationPoints += points;
        }
    }
}