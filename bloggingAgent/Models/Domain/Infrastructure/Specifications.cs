using System;
using System.Linq.Expressions;
using BloggingAgent.Models.Enums;

namespace BloggingAgent.Models.Domain.Infrastructure
{
    // BlogPost Specifications
    public class BlogPostByIdSpec : Specification<BlogPost>
    {
        public BlogPostByIdSpec(Guid id)
        {
            SetCriteria(x => x.Id == id);
            AddInclude(x => x.SeoMetadata);
            AddInclude(x => x.Analytics);
            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Comments);
        }
    }

    public class BlogPostBySlugSpec : Specification<BlogPost>
    {
        public BlogPostBySlugSpec(string slug)
        {
            SetCriteria(x => x.Slug == slug);
            AddInclude(x => x.SeoMetadata);
            AddInclude(x => x.Analytics);
            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Comments);
        }
    }

    public class PublishedBlogPostsSpec : Specification<BlogPost>
    {
        public PublishedBlogPostsSpec(int page = 1, int pageSize = 10)
        {
            SetCriteria(x => x.Status == PostStatus.Published);
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    public class BlogPostsByCategorySpec : Specification<BlogPost>
    {
        public BlogPostsByCategorySpec(BlogCategory category, int page = 1, int pageSize = 10)
        {
            SetCriteria(x => x.Category == category && x.Status == PostStatus.Published);
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    public class BlogPostsByTagSpec : Specification<BlogPost>
    {
        public BlogPostsByTagSpec(string tag, int page = 1, int pageSize = 10)
        {
            SetCriteria(x => x.Tags.Contains(tag) && x.Status == PostStatus.Published);
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    public class BlogPostsByAuthorSpec : Specification<BlogPost>
    {
        public BlogPostsByAuthorSpec(string authorId, int page = 1, int pageSize = 10)
        {
            SetCriteria(x => x.AuthorUserId == authorId && x.Status == PostStatus.Published);
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    public class SearchBlogPostsSpec : Specification<BlogPost>
    {
        public SearchBlogPostsSpec(string searchTerm, int page = 1, int pageSize = 10)
        {
            SetCriteria(x => x.Status == PostStatus.Published &&
                           (x.Title.Contains(searchTerm) ||
                            x.Content.Contains(searchTerm) ||
                            x.Excerpt.Contains(searchTerm) ||
                            x.Tags.Any(tag => tag.Contains(searchTerm))));
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    public class RecentBlogPostsSpec : Specification<BlogPost>
    {
        public RecentBlogPostsSpec(int count = 5)
        {
            SetCriteria(x => x.Status == PostStatus.Published);
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging(0, count);

            AddInclude(x => x.AuthorUser);
        }
    }

    public class PopularBlogPostsSpec : Specification<BlogPost>
    {
        public PopularBlogPostsSpec(int days = 30, int count = 10)
        {
            var sinceDate = DateTime.UtcNow.AddDays(-days);
            SetCriteria(x => x.Status == PostStatus.Published &&
                           x.CreatedAt >= sinceDate);
            ApplyOrderByDescending(x => x.Analytics.Views);
            ApplyPaging(0, count);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    // Comment Specifications
    public class CommentsByBlogPostSpec : Specification<Comment>
    {
        public CommentsByBlogPostSpec(Guid blogPostId, bool approvedOnly = true)
        {
            var criteria = approvedOnly
                ? (Expression<Func<Comment, bool>>)(x => x.BlogPostId == blogPostId && x.Status == CommentStatus.Approved)
                : (Expression<Func<Comment, bool>>)(x => x.BlogPostId == blogPostId);

            SetCriteria(criteria);
            ApplyOrderBy(x => x.CreatedAt);

            AddInclude(x => x.Replies);
        }
    }

    public class PendingCommentsSpec : Specification<Comment>
    {
        public PendingCommentsSpec(int page = 1, int pageSize = 20)
        {
            SetCriteria(x => x.Status == CommentStatus.Pending);
            ApplyOrderBy(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.BlogPost);
        }
    }

    public class SpamCommentsSpec : Specification<Comment>
    {
        public SpamCommentsSpec(int page = 1, int pageSize = 20)
        {
            SetCriteria(x => x.Status == CommentStatus.Spam);
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging((page - 1) * pageSize, pageSize);

            AddInclude(x => x.BlogPost);
        }
    }

    // User Specifications
    public class UserByIdSpec : Specification<ApplicationUser>
    {
        public UserByIdSpec(string userId)
        {
            SetCriteria(x => x.Id.ToString() == userId);
        }
    }

    public class UserByEmailSpec : Specification<ApplicationUser>
    {
        public UserByEmailSpec(string email)
        {
            SetCriteria(x => x.Email == email);
        }
    }

    public class ActiveUsersSpec : Specification<ApplicationUser>
    {
        public ActiveUsersSpec(int page = 1, int pageSize = 20)
        {
            SetCriteria(x => x.IsActive);
            ApplyOrderByDescending(x => x.LastLoginAt);
            ApplyPaging((page - 1) * pageSize, pageSize);
        }
    }

    public class AuthorsByPostCountSpec : Specification<ApplicationUser>
    {
        public AuthorsByPostCountSpec(int minPosts = 1, int count = 10)
        {
            SetCriteria(x => x.IsActive && x.PostsCount >= minPosts);
            ApplyOrderByDescending(x => x.PostsCount);
            ApplyPaging(0, count);
        }
    }

    // Analytics Specifications
    public class AnalyticsByBlogPostSpec : Specification<ContentAnalytics>
    {
        public AnalyticsByBlogPostSpec(Guid blogPostId)
        {
            SetCriteria(x => x.BlogPostId == blogPostId);
        }
    }

    public class TopViewedPostsSpec : Specification<ContentAnalytics>
    {
        public TopViewedPostsSpec(int days = 30, int count = 10)
        {
            var sinceDate = DateTime.UtcNow.AddDays(-days);
            SetCriteria(x => x.UpdatedAt >= sinceDate);
            ApplyOrderByDescending(x => x.Views);
            ApplyPaging(0, count);

            // Note: Navigation property removed - access BlogPost through BlogPostId if needed
        }
    }

    public class AnalyticsSummarySpec : Specification<ContentAnalytics>
    {
        public AnalyticsSummarySpec(DateTime startDate, DateTime endDate)
        {
            SetCriteria(x => x.UpdatedAt >= startDate && x.UpdatedAt <= endDate);
            // Note: Navigation property removed - access BlogPost through BlogPostId if needed
        }
    }

    // Composite Specifications
    public class FeaturedBlogPostsSpec : Specification<BlogPost>
    {
        public FeaturedBlogPostsSpec(int count = 6)
        {
            // Featured posts: published, high view count, recent
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            SetCriteria(x => x.Status == PostStatus.Published &&
                           x.CreatedAt >= oneMonthAgo &&
                           x.Analytics.Views > 10); // Arbitrary threshold
            ApplyOrderByDescending(x => x.Analytics.Views);
            ApplyPaging(0, count);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }

    public class RelatedBlogPostsSpec : Specification<BlogPost>
    {
        public RelatedBlogPostsSpec(Guid blogPostId, BlogCategory category, string[] tags, int count = 5)
        {
            SetCriteria(x => x.Id != blogPostId &&
                           x.Status == PostStatus.Published &&
                           (x.Category == category || x.Tags.Any(tag => tags.Contains(tag))));
            ApplyOrderByDescending(x => x.CreatedAt);
            ApplyPaging(0, count);

            AddInclude(x => x.AuthorUser);
            AddInclude(x => x.Analytics);
        }
    }
}