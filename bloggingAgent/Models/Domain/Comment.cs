using System;
using System.ComponentModel.DataAnnotations;
using BloggingAgent.Models.Enums;

namespace BloggingAgent.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; }

        [StringLength(100)]
        public string AuthorName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string AuthorEmail { get; set; }

        public string AuthorWebsite { get; set; }

        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public string? UserId { get; set; }

        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        public int? ParentCommentId { get; set; }
        public virtual Comment ParentComment { get; set; }

        public virtual System.Collections.Generic.ICollection<Comment> Replies { get; set; } = new System.Collections.Generic.List<Comment>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsSpam { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;

        public int Depth { get; set; } = 0;

        public string TimeAgo => GetTimeAgo();
        public bool CanEdit => !IsDeleted && UpdatedAt == null && (DateTime.UtcNow - CreatedAt).TotalMinutes < 30;
        public bool CanDelete => !IsDeleted;

        public CommentStatus Status { get; set; } = CommentStatus.Pending;

        public void Approve(string approvedBy = null)
        {
            IsApproved = true;
            Status = CommentStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject(string reason = null, string rejectedBy = null)
        {
            IsApproved = false;
            Status = CommentStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsSpam(string markedBy = null)
        {
            IsSpam = true;
            IsApproved = false;
            Status = CommentStatus.Spam;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Like(string userId = null)
        {
            LikesCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Dislike(string userId = null)
        {
            DislikesCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveLike(string userId = null)
        {
            if (LikesCount > 0)
                LikesCount--;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsReply() => ParentCommentId.HasValue;

        public bool IsPending() => Status == CommentStatus.Pending;

        private string GetTimeAgo()
        {
            var timeSpan = DateTime.UtcNow - CreatedAt;
            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
            return CreatedAt.ToString("MMM dd, yyyy");
        }
    }

    public enum CommentStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Spam = 3
    }
}
