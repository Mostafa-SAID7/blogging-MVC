using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<CommentLike> _commentLikeRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICacheService _cacheService;

        public CommentController(
            IRepository<Comment> commentRepository,
            IRepository<CommentLike> commentLikeRepository,
            IRepository<BlogPost> blogPostRepository,
            UserManager<ApplicationUser> userManager,
            ICacheService cacheService)
        {
            _commentRepository = commentRepository;
            _commentLikeRepository = commentLikeRepository;
            _blogPostRepository = blogPostRepository;
            _userManager = userManager;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(Guid postId, int page = 1, int pageSize = 10)
        {
            var cacheKey = $"comments_{postId}_{page}_{pageSize}";
            var cachedComments = await _cacheService.GetAsync<List<CommentDto>>(cacheKey);

            if (cachedComments != null)
            {
                return Json(cachedComments);
            }

            var comments = await _commentRepository.FindAsync(c =>
                c.BlogPostId == postId && c.IsApproved && !c.IsDeleted && !c.IsSpam);

            var commentDtos = comments
                .Where(c => !c.ParentCommentId.HasValue) // Only top-level comments
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => MapToDto(c, includeReplies: true))
                .ToList();

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, commentDtos, TimeSpan.FromMinutes(5));

            return Json(commentDtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            var post = await _blogPostRepository.GetByIdAsync(request.BlogPostId);

            if (post == null)
            {
                return NotFound("Blog post not found");
            }

            var comment = new Comment
            {
                BlogPostId = request.BlogPostId,
                AuthorId = user.Id,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IsApproved = true, // Auto-approve for authenticated users
            };

            await _commentRepository.AddAsync(comment);

            // Clear comment cache
            await ClearCommentCache(request.BlogPostId);

            // Create notification for post author if it's not a reply to their own comment
            if (post.Author != user.UserName && (!request.ParentCommentId.HasValue ||
                (await _commentRepository.GetByIdAsync(request.ParentCommentId.Value))?.AuthorId != post.AuthorId))
            {
                await NotificationController.CreateNotificationAsync(
                    _commentRepository as IRepository<Notification>,
                    _cacheService,
                    new CreateNotificationRequest
                    {
                        UserId = post.AuthorId,
                        Title = "New Comment",
                        Message = $"{user.UserName} commented on your post: {post.Title}",
                        Type = "comment",
                        RelatedEntityType = "BlogPost",
                        RelatedEntityId = post.Id.ToString(),
                        ActionUrl = $"/Blog/Details/{post.Slug}#comment-{comment.Id}"
                    });
            }

            return Json(new { success = true, commentId = comment.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeComment(Guid commentId, bool isLike)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null || comment.IsDeleted)
            {
                return NotFound();
            }

            if (isLike)
            {
                comment.Like(user.Id);
            }
            else
            {
                comment.Dislike(user.Id);
            }

            await _commentRepository.UpdateAsync(comment);

            // Clear comment cache
            await ClearCommentCache(comment.BlogPostId);

            return Json(new
            {
                success = true,
                likesCount = comment.LikesCount,
                dislikesCount = comment.DislikesCount
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveCommentLike(Guid commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            comment.RemoveLike(user.Id);
            await _commentRepository.UpdateAsync(comment);

            // Clear comment cache
            await ClearCommentCache(comment.BlogPostId);

            return Json(new
            {
                success = true,
                likesCount = comment.LikesCount,
                dislikesCount = comment.DislikesCount
            });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> ModerateComment(Guid commentId, string action, string reason = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            switch (action.ToLower())
            {
                case "approve":
                    comment.Approve(user.UserName);
                    break;
                case "reject":
                    comment.Reject(reason, user.UserName);
                    break;
                case "spam":
                    comment.MarkAsSpam(user.UserName);
                    break;
                case "delete":
                    comment.SoftDelete();
                    break;
                default:
                    return BadRequest("Invalid action");
            }

            await _commentRepository.UpdateAsync(comment);

            // Clear comment cache
            await ClearCommentCache(comment.BlogPostId);

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditComment(Guid commentId, [FromBody] string content)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null || comment.AuthorId != user.Id || !comment.CanEdit)
            {
                return BadRequest("Cannot edit this comment");
            }

            comment.Content = content;
            comment.MarkAsModified();

            await _commentRepository.UpdateAsync(comment);

            // Clear comment cache
            await ClearCommentCache(comment.BlogPostId);

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null || comment.AuthorId != user.Id)
            {
                return BadRequest("Cannot delete this comment");
            }

            comment.SoftDelete();
            await _commentRepository.UpdateAsync(comment);

            // Clear comment cache
            await ClearCommentCache(comment.BlogPostId);

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> PendingComments(int page = 1, int pageSize = 20)
        {
            var pendingComments = await _commentRepository.FindAsync(c =>
                !c.IsApproved && !c.IsSpam && !c.IsDeleted);

            var comments = pendingComments
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => MapToDto(c, includeReplies: false))
                .ToList();

            return View(comments);
        }

        [HttpGet]
        public async Task<IActionResult> CommentStats(Guid postId)
        {
            var cacheKey = $"comment_stats_{postId}";
            var cachedStats = await _cacheService.GetAsync<CommentStatsDto>(cacheKey);

            if (cachedStats != null)
            {
                return Json(cachedStats);
            }

            var comments = await _commentRepository.FindAsync(c =>
                c.BlogPostId == postId && c.IsApproved && !c.IsDeleted && !c.IsSpam);

            var stats = new CommentStatsDto
            {
                TotalComments = comments.Count(),
                TotalLikes = comments.Sum(c => c.LikesCount),
                TotalReplies = comments.Count(c => c.IsReply()),
                RecentComments = comments.Count(c => c.CreatedAt > DateTime.UtcNow.AddDays(-7))
            };

            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(10));

            return Json(stats);
        }

        private CommentDto MapToDto(Comment comment, bool includeReplies = false)
        {
            var dto = new CommentDto
            {
                Id = comment.Id,
                BlogPostId = comment.BlogPostId,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author?.UserName ?? comment.AuthorName,
                Content = comment.Content,
                IsApproved = comment.IsApproved,
                LikesCount = comment.LikesCount,
                DislikesCount = comment.DislikesCount,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                TimeAgo = comment.TimeAgo,
                CanEdit = comment.CanEdit,
                CanDelete = comment.CanDelete,
                Depth = comment.Depth,
                IsReply = comment.IsReply(),
                ParentCommentId = comment.ParentCommentId
            };

            if (includeReplies && comment.Replies.Any())
            {
                dto.Replies = comment.Replies
                    .Where(r => r.IsApproved && !r.IsDeleted && !r.IsSpam)
                    .OrderBy(r => r.CreatedAt)
                    .Select(r => MapToDto(r, includeReplies: true))
                    .ToList();
            }

            return dto;
        }

        private async Task ClearCommentCache(Guid postId)
        {
            // Clear all comment-related caches for this post
            await _cacheService.RemoveAsync($"comments_{postId}_*");
            await _cacheService.RemoveAsync($"comment_stats_{postId}");
        }
    }

}