using System;

namespace BloggingAgent.Models.Domain
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
        public int BlogPostId { get; }
        public string Title { get; }
        public string Author { get; }

        public BlogPostCreatedEvent(int blogPostId, string title, string author)
        {
            BlogPostId = blogPostId;
            Title = title;
            Author = author;
        }
    }

    public class BlogPostPublishedEvent : DomainEvent
    {
        public int BlogPostId { get; }
        public string Title { get; }
        public DateTime PublishedAt { get; }

        public BlogPostPublishedEvent(int blogPostId, string title, DateTime publishedAt)
        {
            BlogPostId = blogPostId;
            Title = title;
            PublishedAt = publishedAt;
        }
    }

    public class BlogPostUpdatedEvent : DomainEvent
    {
        public int BlogPostId { get; }
        public string Title { get; }
        public string[] ModifiedFields { get; }

        public BlogPostUpdatedEvent(int blogPostId, string title, string[] modifiedFields)
        {
            BlogPostId = blogPostId;
            Title = title;
            ModifiedFields = modifiedFields;
        }
    }

    public class BlogPostDeletedEvent : DomainEvent
    {
        public int BlogPostId { get; }
        public string Title { get; }
        public string DeletedBy { get; }

        public BlogPostDeletedEvent(int blogPostId, string title, string deletedBy)
        {
            BlogPostId = blogPostId;
            Title = title;
            DeletedBy = deletedBy;
        }
    }

    // Comment Events
    public class CommentAddedEvent : DomainEvent
    {
        public int CommentId { get; }
        public int BlogPostId { get; }
        public string AuthorName { get; }
        public string Content { get; }

        public CommentAddedEvent(int commentId, int blogPostId, string authorName, string content)
        {
            CommentId = commentId;
            BlogPostId = blogPostId;
            AuthorName = authorName;
            Content = content;
        }
    }

    public class CommentApprovedEvent : DomainEvent
    {
        public int CommentId { get; }
        public int BlogPostId { get; }
        public string ApprovedBy { get; }

        public CommentApprovedEvent(int commentId, int blogPostId, string approvedBy)
        {
            CommentId = commentId;
            BlogPostId = blogPostId;
            ApprovedBy = approvedBy;
        }
    }

    public class CommentRejectedEvent : DomainEvent
    {
        public int CommentId { get; }
        public int BlogPostId { get; }
        public string RejectedBy { get; }

        public CommentRejectedEvent(int commentId, int blogPostId, string rejectedBy)
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
        public int BlogPostId { get; }
        public string ViewerIp { get; }
        public string UserAgent { get; }
        public string Referrer { get; }

        public BlogPostViewedEvent(int blogPostId, string viewerIp, string userAgent, string referrer)
        {
            BlogPostId = blogPostId;
            ViewerIp = viewerIp;
            UserAgent = userAgent;
            Referrer = referrer;
        }
    }

    public class BlogPostSharedEvent : DomainEvent
    {
        public int BlogPostId { get; }
        public string Platform { get; } // Twitter, Facebook, LinkedIn, etc.
        public string SharedBy { get; }

        public BlogPostSharedEvent(int blogPostId, string platform, string sharedBy)
        {
            BlogPostId = blogPostId;
            Platform = platform;
            SharedBy = sharedBy;
        }
    }
}