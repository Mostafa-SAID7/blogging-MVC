using System;

namespace BloggingAgent.Models.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        Guid EventId { get; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; protected set; }
        public Guid EventId { get; protected set; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }
    }

    // Blog Post Events
    public class BlogPostCreatedEvent : DomainEvent
    {
        public Guid BlogPostId { get; }
        public string Title { get; }
        public string Author { get; }

        public BlogPostCreatedEvent(Guid blogPostId, string title, string author)
        {
            BlogPostId = blogPostId;
            Title = title;
            Author = author;
        }
    }

    public class BlogPostPublishedEvent : DomainEvent
    {
        public Guid BlogPostId { get; }
        public string Title { get; }
        public DateTime PublishedAt { get; }

        public BlogPostPublishedEvent(Guid blogPostId, string title, DateTime publishedAt)
        {
            BlogPostId = blogPostId;
            Title = title;
            PublishedAt = publishedAt;
        }
    }

    public class BlogPostUpdatedEvent : DomainEvent
    {
        public Guid BlogPostId { get; }
        public string Title { get; }
        public string[] ModifiedFields { get; }

        public BlogPostUpdatedEvent(Guid blogPostId, string title, string[] modifiedFields)
        {
            BlogPostId = blogPostId;
            Title = title;
            ModifiedFields = modifiedFields;
        }
    }

    public class BlogPostDeletedEvent : DomainEvent
    {
        public Guid BlogPostId { get; }
        public string Title { get; }
        public string DeletedBy { get; }

        public BlogPostDeletedEvent(Guid blogPostId, string title, string deletedBy)
        {
            BlogPostId = blogPostId;
            Title = title;
            DeletedBy = deletedBy;
        }
    }

    // Comment Events
    public class CommentAddedEvent : DomainEvent
    {
        public Guid CommentId { get; }
        public Guid BlogPostId { get; }
        public string AuthorName { get; }
        public string Content { get; }

        public CommentAddedEvent(Guid commentId, Guid blogPostId, string authorName, string content)
        {
            CommentId = commentId;
            BlogPostId = blogPostId;
            AuthorName = authorName;
            Content = content;
        }
    }

    public class CommentApprovedEvent : DomainEvent
    {
        public Guid CommentId { get; }
        public Guid BlogPostId { get; }
        public string ApprovedBy { get; }

        public CommentApprovedEvent(Guid commentId, Guid blogPostId, string approvedBy)
        {
            CommentId = commentId;
            BlogPostId = blogPostId;
            ApprovedBy = approvedBy;
        }
    }

    public class CommentRejectedEvent : DomainEvent
    {
        public Guid CommentId { get; }
        public Guid BlogPostId { get; }
        public string RejectedBy { get; }

        public CommentRejectedEvent(Guid commentId, Guid blogPostId, string rejectedBy)
        {
            CommentId = commentId;
            BlogPostId = blogPostId;
            RejectedBy = rejectedBy;
        }
    }

    // User Events
    public class UserRegisteredEvent : DomainEvent
    {
        public string UserId { get; }
        public string Email { get; }
        public string UserName { get; }

        public UserRegisteredEvent(string userId, string email, string userName)
        {
            UserId = userId;
            Email = email;
            UserName = userName;
        }
    }

    public class UserLoginEvent : DomainEvent
    {
        public string UserId { get; }
        public string Email { get; }
        public DateTime LoginTime { get; }

        public UserLoginEvent(string userId, string email, DateTime loginTime)
        {
            UserId = userId;
            Email = email;
            LoginTime = loginTime;
        }
    }

    // Analytics Events
    public class BlogPostViewedEvent : DomainEvent
    {
        public Guid BlogPostId { get; }
        public string ViewerIp { get; }
        public string UserAgent { get; }
        public string Referrer { get; }

        public BlogPostViewedEvent(Guid blogPostId, string viewerIp, string userAgent, string referrer)
        {
            BlogPostId = blogPostId;
            ViewerIp = viewerIp;
            UserAgent = userAgent;
            Referrer = referrer;
        }
    }

    public class BlogPostSharedEvent : DomainEvent
    {
        public Guid BlogPostId { get; }
        public string Platform { get; } // Twitter, Facebook, LinkedIn, etc.
        public string SharedBy { get; }

        public BlogPostSharedEvent(Guid blogPostId, string platform, string sharedBy)
        {
            BlogPostId = blogPostId;
            Platform = platform;
            SharedBy = sharedBy;
        }
    }
}