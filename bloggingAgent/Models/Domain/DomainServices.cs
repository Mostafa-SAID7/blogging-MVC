using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloggingAgent.Models.Domain
{
    // Domain Service interfaces
    public interface IBlogPostDomainService
    {
        Task<Result<BlogPost>> CreateBlogPostAsync(string title, string content, string author, BlogCategory category = BlogCategory.Other);
        Task<Result> UpdateBlogPostAsync(BlogPost blogPost, string newContent, string newExcerpt = null);
        Task<Result> PublishBlogPostAsync(BlogPost blogPost);
        Task<Result> ArchiveBlogPostAsync(BlogPost blogPost);
        Task<Result<int>> CalculateReadingTimeAsync(string content);
        Task<Result<string>> GenerateExcerptAsync(string content, int maxLength = 150);
        Task<Result<bool>> IsContentOriginalAsync(string content, string author);
    }

    public interface ICommentDomainService
    {
        Task<Result<Comment>> CreateCommentAsync(int blogPostId, string content, string authorName, string authorEmail = null);
        Task<Result> ApproveCommentAsync(Comment comment);
        Task<Result> RejectCommentAsync(Comment comment);
        Task<Result<bool>> IsCommentSpamAsync(Comment comment);
        Task<Result<IEnumerable<Comment>>> GetApprovedCommentsAsync(int blogPostId);
    }

    public interface IUserDomainService
    {
        Task<Result<ApplicationUser>> CreateUserAsync(string email, string userName, string firstName = null, string lastName = null);
        Task<Result> UpdateUserProfileAsync(ApplicationUser user, string bio, string avatarUrl = null);
        Task<Result<bool>> CanUserPublishAsync(ApplicationUser user);
        Task<Result<int>> GetUserPostCountAsync(string userId);
    }

    public interface ISeoDomainService
    {
        Task<Result<SeoMetadata>> GenerateSeoMetadataAsync(string title, string content, List<string> keywords = null);
        Task<Result<int>> CalculateSeoScoreAsync(string content, string title = null);
        Task<Result<List<string>>> SuggestKeywordsAsync(string content, int count = 5);
        Task<Result<bool>> IsContentSearchOptimizedAsync(string content, List<string> targetKeywords);
    }

    // Domain Service implementations
    public class BlogPostDomainService : IBlogPostDomainService
    {
        private readonly IDomainEventDispatcher _eventDispatcher;

        public BlogPostDomainService(IDomainEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        }

        public async Task<Result<BlogPost>> CreateBlogPostAsync(string title, string content, string author, BlogCategory category = BlogCategory.Other)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(title))
                    return Result<BlogPost>.Failure("Title is required", "VALIDATION_ERROR", 400);

                if (string.IsNullOrWhiteSpace(content))
                    return Result<BlogPost>.Failure("Content is required", "VALIDATION_ERROR", 400);

                if (string.IsNullOrWhiteSpace(author))
                    return Result<BlogPost>.Failure("Author is required", "VALIDATION_ERROR", 400);

                // Create blog post
                var blogPost = new BlogPost
                {
                    Title = title.Trim(),
                    Content = content.Trim(),
                    Author = author.Trim(),
                    Category = category,
                    Status = PostStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Calculate derived properties
                blogPost.WordCount = CalculateWordCount(content);
                blogPost.ReadingTimeMinutes = CalculateReadingTime(blogPost.WordCount);
                blogPost.Excerpt = await GenerateExcerptAsync(content);

                // Generate slug
                blogPost.Slug = Slug.GenerateFromTitle(title).Value;

                // Raise domain event
                var createdEvent = new BlogPostCreatedEvent(blogPost.Id, blogPost.Title, blogPost.Author);
                blogPost.AddDomainEvent(createdEvent);

                await _eventDispatcher.DispatchAsync(createdEvent);

                return Result<BlogPost>.Success(blogPost);
            }
            catch (Exception ex)
            {
                return Result<BlogPost>.Failure(ex, "BLOG_POST_CREATION_FAILED", 500);
            }
        }

        public async Task<Result> UpdateBlogPostAsync(BlogPost blogPost, string newContent, string newExcerpt = null)
        {
            try
            {
                if (blogPost == null)
                    return Result.Failure("Blog post not found", "NOT_FOUND", 404);

                // Validate business rules
                if (blogPost.Status == PostStatus.Archived)
                    return Result.Failure("Cannot update archived blog post", "BUSINESS_RULE_VIOLATION", 400);

                // Update content
                blogPost.UpdateContent(newContent, newExcerpt);

                // Raise domain event
                var updatedEvent = new BlogPostUpdatedEvent(
                    blogPost.Id,
                    blogPost.Title,
                    new[] { "Content", "WordCount", "ReadingTimeMinutes" });

                blogPost.AddDomainEvent(updatedEvent);
                await _eventDispatcher.DispatchAsync(updatedEvent);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex, "BLOG_POST_UPDATE_FAILED", 500);
            }
        }

        public async Task<Result> PublishBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                if (blogPost == null)
                    return Result.Failure("Blog post not found", "NOT_FOUND", 404);

                blogPost.Publish();

                // Raise domain event
                var publishedEvent = new BlogPostPublishedEvent(blogPost.Id, blogPost.Title, blogPost.UpdatedAt);
                blogPost.AddDomainEvent(publishedEvent);
                await _eventDispatcher.DispatchAsync(publishedEvent);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex);
            }
            catch (Exception ex)
            {
                return Result.Failure(ex, "BLOG_POST_PUBLISH_FAILED", 500);
            }
        }

        public async Task<Result> ArchiveBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                if (blogPost == null)
                    return Result.Failure("Blog post not found", "NOT_FOUND", 404);

                blogPost.Archive();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex, "BLOG_POST_ARCHIVE_FAILED", 500);
            }
        }

        public Task<Result<int>> CalculateReadingTimeAsync(string content)
        {
            const int wordsPerMinute = 200;
            var wordCount = CalculateWordCount(content);
            var readingTime = Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));

            return Task.FromResult(Result<int>.Success(readingTime));
        }

        public Task<Result<string>> GenerateExcerptAsync(string content, int maxLength = 150)
        {
            if (string.IsNullOrEmpty(content))
                return Task.FromResult(Result<string>.Success(string.Empty));

            var excerpt = content.Length <= maxLength
                ? content
                : content.Substring(0, maxLength).TrimEnd() + "...";

            return Task.FromResult(Result<string>.Success(excerpt));
        }

        public Task<Result<bool>> IsContentOriginalAsync(string content, string author)
        {
            // This would integrate with plagiarism detection services
            // For now, return true (assuming content is original)
            return Task.FromResult(Result<bool>.Success(true));
        }

        private static int CalculateWordCount(string content)
        {
            if (string.IsNullOrEmpty(content)) return 0;

            var words = content.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        private static int CalculateReadingTime(int wordCount)
        {
            const int wordsPerMinute = 200;
            return Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));
        }
    }

    // Business Rules
    public class BlogPostTitleNotEmptyRule : BusinessRule
    {
        private readonly string _title;

        public BlogPostTitleNotEmptyRule(string title)
        {
            _title = title;
        }

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_title);

        public override string Message => "Blog post title cannot be empty";
    }

    public class BlogPostContentNotEmptyRule : BusinessRule
    {
        private readonly string _content;

        public BlogPostContentNotEmptyRule(string content)
        {
            _content = content;
        }

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_content);

        public override string Message => "Blog post content cannot be empty";
    }

    public class BlogPostAuthorNotEmptyRule : BusinessRule
    {
        private readonly string _author;

        public BlogPostAuthorNotEmptyRule(string author)
        {
            _author = author;
        }

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_author);

        public override string Message => "Blog post author cannot be empty";
    }

    public class CommentContentNotEmptyRule : BusinessRule
    {
        private readonly string _content;

        public CommentContentNotEmptyRule(string content)
        {
            _content = content;
        }

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_content);

        public override string Message => "Comment content cannot be empty";
    }

    public class CommentAuthorNotEmptyRule : BusinessRule
    {
        private readonly string _authorName;

        public CommentAuthorNotEmptyRule(string authorName)
        {
            _authorName = authorName;
        }

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_authorName);

        public override string Message => "Comment author name cannot be empty";
    }
}