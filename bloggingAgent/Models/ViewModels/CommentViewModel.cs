using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Models.ViewModels
{
    public class CommentViewModel
    {
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public int BlogPostId { get; set; }
        public bool CanComment { get; set; }
        public bool RequireModeration { get; set; }
        public int TotalComments { get; set; }
        public int ApprovedComments { get; set; }
        public int PendingComments { get; set; }
    }

    public class CommentFormViewModel
    {
        public int BlogPostId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
        public bool IsAnonymous { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public bool RememberMe { get; set; }
    }

    public class CommentModerationViewModel
    {
        public List<CommentDto> PendingComments { get; set; } = new List<CommentDto>();
        public List<CommentDto> ApprovedComments { get; set; } = new List<CommentDto>();
        public List<CommentDto> SpamComments { get; set; } = new List<CommentDto>();
        public int TotalPending { get; set; }
        public int TotalApproved { get; set; }
        public int TotalSpam { get; set; }
    }
}