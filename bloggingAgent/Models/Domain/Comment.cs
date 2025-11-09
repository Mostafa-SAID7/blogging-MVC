using System;
using System.ComponentModel.DataAnnotations;
using BloggingAgent.Models.Enums;
using BloggingAgent.Models.ValueObjects;

namespace BloggingAgent.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; }

        [Required]
        [StringLength(100)]
        public string AuthorName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string AuthorEmail { get; set; }

        public string AuthorWebsite { get; set; }

        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        public int? ParentCommentId { get; set; }
        public virtual Comment ParentComment { get; set; }

        public virtual System.Collections.Generic.ICollection<Comment> Replies { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CommentStatus Status { get; set; } = CommentStatus.Pending;

        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        // Domain methods
        public void Approve()
        {
            if (Status != CommentStatus.Pending)
                throw new InvalidOperationException("Only pending comments can be approved");

            Status = CommentStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject()
        {
            if (Status != CommentStatus.Pending)
                throw new InvalidOperationException("Only pending comments can be rejected");

            Status = CommentStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsSpam()
        {
            Status = CommentStatus.Spam;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsApproved() => Status == CommentStatus.Approved;

        public bool IsPending() => Status == CommentStatus.Pending;

        public bool IsSpam() => Status == CommentStatus.Spam;

        public bool IsReply() => ParentCommentId.HasValue;
    }

    public enum CommentStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Spam = 3
    }
}