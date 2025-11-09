using System;

namespace BloggingAgent.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public bool IsSpam { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        // Navigation properties
        public BlogPost BlogPost { get; set; }
        public User User { get; set; }
        public Comment ParentComment { get; set; }
        public int? ParentCommentId { get; set; }
    }
}