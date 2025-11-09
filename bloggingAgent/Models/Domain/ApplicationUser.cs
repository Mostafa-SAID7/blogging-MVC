e.fusing Microsoft.AspNetCore.Identity;
using System;

namespace BloggingAgent.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string PreferredLanguage { get; set; } = "en";
        public string TimeZone { get; set; } = "UTC";
        public int PostsCount { get; set; } = 0;
        public DateTime? LastPostDate { get; set; }

        // Navigation properties
        public virtual System.Collections.Generic.ICollection<BlogPost> BlogPosts { get; set; }

        public string GetFullName()
        {
            return string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
                ? UserName
                : $"{FirstName} {LastName}".Trim();
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void IncrementPostsCount()
        {
            PostsCount++;
            LastPostDate = DateTime.UtcNow;
        }
    }
}