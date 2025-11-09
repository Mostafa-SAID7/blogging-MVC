namespace BloggingAgent.Models.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsApproved { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
        public int Depth { get; set; } // For nested display
    }

    public class CreateCommentRequest
    {
        public string Content { get; set; }
        public int BlogPostId { get; set; }
        public int? ParentCommentId { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public string AuthorName { get; set; } // For anonymous comments
        public string AuthorEmail { get; set; } // For anonymous comments
    }

    public class UpdateCommentRequest
    {
        public string Content { get; set; }
    }

    public class CommentModerationRequest
    {
        public bool IsApproved { get; set; }
        public bool IsSpam { get; set; }
    }
}