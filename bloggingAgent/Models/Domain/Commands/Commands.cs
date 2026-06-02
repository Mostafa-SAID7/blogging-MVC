using System;
using System.Collections.Generic;
using BloggingAgent.Models.Domain.Infrastructure;
using BloggingAgent.Models.Enums;

namespace BloggingAgent.Models.Domain.Commands
{
    // Command base classes
    public abstract class Command
    {
        public Guid CommandId { get; } = Guid.NewGuid();
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string UserId { get; set; }
    }

    public abstract class Command<TResponse> : Command
    {
    }

    // BlogPost Commands
    public class CreateBlogPostCommand : Command<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string Author { get; set; }
        public BlogCategory Category { get; set; } = BlogCategory.Other;
        public ContentTone Tone { get; set; } = ContentTone.Professional;
        public List<string> Tags { get; set; } = new List<string>();
        public bool PublishImmediately { get; set; } = false;
    }

    public class UpdateBlogPostCommand : Command
    {
        public int BlogPostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public BlogCategory Category { get; set; }
        public ContentTone Tone { get; set; }
        public List<string> Tags { get; set; }
    }

    public class PublishBlogPostCommand : Command
    {
        public int BlogPostId { get; set; }
    }

    public class UnpublishBlogPostCommand : Command
    {
        public int BlogPostId { get; set; }
    }

    public class ArchiveBlogPostCommand : Command
    {
        public int BlogPostId { get; set; }
    }

    public class DeleteBlogPostCommand : Command
    {
        public int BlogPostId { get; set; }
    }

    public class GenerateBlogPostCommand : Command<int>
    {
        public string Topic { get; set; }
        public string Keywords { get; set; }
        public int TargetWordCount { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public ContentTone Tone { get; set; } = ContentTone.Professional;
        public string TargetAudience { get; set; }
        public bool IncludeImages { get; set; } = false;
    }

    // Comment Commands
    public class AddCommentCommand : Command<int>
    {
        public int BlogPostId { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorWebsite { get; set; }
        public int? ParentCommentId { get; set; }
    }

    public class ApproveCommentCommand : Command
    {
        public int CommentId { get; set; }
    }

    public class RejectCommentCommand : Command
    {
        public int CommentId { get; set; }
    }

    public class MarkCommentAsSpamCommand : Command
    {
        public int CommentId { get; set; }
    }

    public class DeleteCommentCommand : Command
    {
        public int CommentId { get; set; }
    }

    // User Commands
    public class CreateUserCommand : Command<string>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserProfileCommand : Command
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }
    }

    public class ChangeUserPasswordCommand : Command
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class DeleteUserCommand : Command
    {
        public string UserId { get; set; }
    }

    // Analytics Commands
    public class TrackBlogPostViewCommand : Command
    {
        public int BlogPostId { get; set; }
        public string ViewerIp { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public string UserId { get; set; }
    }

    public class TrackBlogPostShareCommand : Command
    {
        public int BlogPostId { get; set; }
        public string Platform { get; set; }
        public string UserId { get; set; }
    }

    // SEO Commands
    public class OptimizeBlogPostSeoCommand : Command
    {
        public int BlogPostId { get; set; }
        public List<string> TargetKeywords { get; set; } = new List<string>();
        public bool AutoGenerateMeta { get; set; } = true;
    }

    public class UpdateBlogPostSeoCommand : Command
    {
        public int BlogPostId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string FocusKeywords { get; set; }
        public bool NoIndex { get; set; } = false;
    }

    // Bulk Operations Commands
    public class BulkPublishBlogPostsCommand : Command<int>
    {
        public List<int> BlogPostIds { get; set; } = new List<int>();
    }

    public class BulkArchiveBlogPostsCommand : Command<int>
    {
        public List<int> BlogPostIds { get; set; } = new List<int>();
    }

    public class BulkDeleteBlogPostsCommand : Command<int>
    {
        public List<int> BlogPostIds { get; set; } = new List<int>();
    }

    public class BulkApproveCommentsCommand : Command<int>
    {
        public List<int> CommentIds { get; set; } = new List<int>();
    }

    public class BulkMarkCommentsAsSpamCommand : Command<int>
    {
        public List<int> CommentIds { get; set; } = new List<int>();
    }

    // Settings Commands
    public class UpdateAgentSettingsCommand : Command
    {
        public string DefaultAuthor { get; set; }
        public int MaxPostLength { get; set; }
        public List<string> DefaultTags { get; set; } = new List<string>();
        public bool AutoPublish { get; set; }
        public string Theme { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }

    public class ResetAgentSettingsCommand : Command
    {
    }

    // Import/Export Commands
    public class ImportBlogPostsCommand : Command<int>
    {
        public string ImportData { get; set; }
        public string Format { get; set; } // "json", "xml", "csv"
        public bool PublishImported { get; set; } = false;
    }

    public class ExportBlogPostsCommand : Command<string>
    {
        public List<int> BlogPostIds { get; set; } = new List<int>();
        public string Format { get; set; } = "json";
        public bool IncludeComments { get; set; } = true;
        public bool IncludeAnalytics { get; set; } = true;
    }

    // Search Commands
    public class SearchBlogPostsCommand : Command<List<BlogPost>>
    {
        public string Query { get; set; }
        public BlogCategory? Category { get; set; }
        public string Tag { get; set; }
        public string Author { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
    }

    public class GetBlogPostSuggestionsCommand : Command<List<BlogPost>>
    {
        public string CurrentContent { get; set; }
        public int MaxSuggestions { get; set; } = 5;
    }
}