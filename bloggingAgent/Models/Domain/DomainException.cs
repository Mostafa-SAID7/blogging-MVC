using System;

namespace BloggingAgent.Models.Domain
{
    public abstract class DomainException : Exception
    {
        public string ErrorCode { get; }
        public object[] Parameters { get; }

        protected DomainException(string message, string errorCode, params object[] parameters)
            : base(message)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        protected DomainException(string message, Exception innerException, string errorCode, params object[] parameters)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }
    }

    public class BlogPostDomainException : DomainException
    {
        public BlogPostDomainException(string message, string errorCode = null, params object[] parameters)
            : base(message, errorCode ?? "BLOG_POST_ERROR", parameters)
        {
        }

        public static BlogPostDomainException InvalidStatusTransition(PostStatus currentStatus, PostStatus targetStatus)
        {
            return new BlogPostDomainException(
                $"Cannot transition from {currentStatus} to {targetStatus}",
                "INVALID_STATUS_TRANSITION",
                currentStatus,
                targetStatus);
        }

        public static BlogPostDomainException EmptyContent()
        {
            return new BlogPostDomainException(
                "Blog post content cannot be empty",
                "EMPTY_CONTENT");
        }

        public static BlogPostDomainException InvalidTitle()
        {
            return new BlogPostDomainException(
                "Blog post title is required and cannot be empty",
                "INVALID_TITLE");
        }

        public static BlogPostDomainException DuplicateSlug(string slug)
        {
            return new BlogPostDomainException(
                $"Slug '{slug}' already exists",
                "DUPLICATE_SLUG",
                slug);
        }
    }

    public class CommentDomainException : DomainException
    {
        public CommentDomainException(string message, string errorCode = null, params object[] parameters)
            : base(message, errorCode ?? "COMMENT_ERROR", parameters)
        {
        }

        public static CommentDomainException InvalidStatusTransition(CommentStatus currentStatus, CommentStatus targetStatus)
        {
            return new CommentDomainException(
                $"Cannot transition comment from {currentStatus} to {targetStatus}",
                "INVALID_COMMENT_STATUS_TRANSITION",
                currentStatus,
                targetStatus);
        }

        public static CommentDomainException EmptyContent()
        {
            return new CommentDomainException(
                "Comment content cannot be empty",
                "EMPTY_COMMENT_CONTENT");
        }

        public static CommentDomainException InvalidAuthor()
        {
            return new CommentDomainException(
                "Comment author name is required",
                "INVALID_COMMENT_AUTHOR");
        }
    }

    public class UserDomainException : DomainException
    {
        public UserDomainException(string message, string errorCode = null, params object[] parameters)
            : base(message, errorCode ?? "USER_ERROR", parameters)
        {
        }

        public static UserDomainException InvalidEmail(string email)
        {
            return new UserDomainException(
                $"Invalid email address: {email}",
                "INVALID_EMAIL",
                email);
        }

        public static UserDomainException DuplicateEmail(string email)
        {
            return new UserDomainException(
                $"Email address already exists: {email}",
                "DUPLICATE_EMAIL",
                email);
        }

        public static UserDomainException AccountLocked(string email)
        {
            return new UserDomainException(
                $"Account is locked: {email}",
                "ACCOUNT_LOCKED",
                email);
        }
    }

    public class ValidationDomainException : DomainException
    {
        public ValidationDomainException(string message, string errorCode = null, params object[] parameters)
            : base(message, errorCode ?? "VALIDATION_ERROR", parameters)
        {
        }

        public static ValidationDomainException RequiredField(string fieldName)
        {
            return new ValidationDomainException(
                $"{fieldName} is required",
                "REQUIRED_FIELD",
                fieldName);
        }

        public static ValidationDomainException InvalidLength(string fieldName, int minLength, int maxLength)
        {
            return new ValidationDomainException(
                $"{fieldName} must be between {minLength} and {maxLength} characters",
                "INVALID_LENGTH",
                fieldName,
                minLength,
                maxLength);
        }

        public static ValidationDomainException InvalidFormat(string fieldName, string expectedFormat)
        {
            return new ValidationDomainException(
                $"{fieldName} has invalid format. Expected: {expectedFormat}",
                "INVALID_FORMAT",
                fieldName,
                expectedFormat);
        }
    }
}