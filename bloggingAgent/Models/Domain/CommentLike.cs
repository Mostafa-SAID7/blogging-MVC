using System;

namespace BloggingAgent.Models.Domain
{
    public class CommentLike : BaseEntity
    {
        // Relationships
        public Guid CommentId { get; set; }
        public virtual Comment Comment { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Like/Dislike
        public bool IsLike { get; set; } // true = like, false = dislike

        // Computed Properties
        public string DisplayText => IsLike ? "Liked" : "Disliked";
        public string IconClass => IsLike ? "fas fa-thumbs-up" : "fas fa-thumbs-down";
        public string ColorClass => IsLike ? "text-success" : "text-danger";
    }
}