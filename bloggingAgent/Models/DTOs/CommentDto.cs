using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string TimeAgo { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public int Depth { get; set; }
        public bool IsReply { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }

    public class CreateCommentRequest
    {
        public int BlogPostId { get; set; }
        public string Content { get; set; }
        public int? ParentCommentId { get; set; }
    }

    public class CommentStatsDto
    {
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
        public int TotalReplies { get; set; }
        public int RecentComments { get; set; }
    }
}