namespace BloggingAgent.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public string AuthorName { get; set; } // For anonymous comments
        public string AuthorEmail { get; set; } // For anonymous comments
        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public int? ParentCommentId { get; set; } // For nested replies
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsApproved { get; set; } = false; // Moderation
        public bool IsSpam { get; set; } = false;
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
    }
}