using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers
{
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            IRepository<Comment> commentRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<User> userRepository,
            ILogger<CommentController> logger)
        {
            _commentRepository = commentRepository;
            _blogPostRepository = blogPostRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int postId, string content, int? parentCommentId = null)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment content cannot be empty.";
                return RedirectToAction("Details", "Blog", new { id = postId });
            }

            var post = await _blogPostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            // For now, create anonymous user or use default user
            // In a real app, this would come from authentication
            var user = await GetOrCreateAnonymousUserAsync();

            var comment = new Comment
            {
                BlogPostId = postId,
                UserId = user.Id,
                Content = content.Trim(),
                ParentCommentId = parentCommentId,
                IsApproved = true, // Auto-approve for now
                CreatedAt = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            await _commentRepository.AddAsync(comment);

            // Update post analytics
            await UpdatePostCommentCountAsync(postId);

            _logger.LogInformation("Comment added to post {PostId} by user {UserId}", postId, user.Id);

            TempData["Success"] = "Comment added successfully.";
            return RedirectToAction("Details", "Blog", new { slug = post.Slug });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsApproved = true;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.UpdateAsync(comment);

            _logger.LogInformation("Comment {CommentId} approved", id);

            return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsSpam(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsSpam = true;
            comment.IsApproved = false;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.UpdateAsync(comment);

            _logger.LogInformation("Comment {CommentId} marked as spam", id);

            return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var postId = comment.BlogPostId;
            await _commentRepository.DeleteAsync(id);

            // Update post analytics
            await UpdatePostCommentCountAsync(postId);

            _logger.LogInformation("Comment {CommentId} deleted", id);

            return RedirectToAction("Details", "Blog", new { id = postId });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await _commentRepository.FindAsync(c => c.BlogPostId == postId && c.IsApproved && !c.IsSpam);
            var commentDtos = comments.Select(MapToDto).ToList();

            return Json(commentDtos);
        }

        private async Task<User> GetOrCreateAnonymousUserAsync()
        {
            // In a real application, this would use authentication
            // For now, create or find an anonymous user
            var anonymousUser = await _userRepository.SingleOrDefaultAsync(u => u.Username == "Anonymous");

            if (anonymousUser == null)
            {
                anonymousUser = new User
                {
                    Username = "Anonymous",
                    Email = "anonymous@bloggingagent.com",
                    FirstName = "Anonymous",
                    LastName = "User",
                    Role = UserRole.Reader,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(anonymousUser);
            }

            return anonymousUser;
        }

        private async Task UpdatePostCommentCountAsync(int postId)
        {
            var commentCount = await _commentRepository.CountAsync(c => c.BlogPostId == postId && c.IsApproved && !c.IsSpam);

            // Update the analytics if available
            // This would be handled by the analytics service in a real implementation
            _logger.LogInformation("Updated comment count for post {PostId}: {Count}", postId, commentCount);
        }

        private CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                BlogPostId = comment.BlogPostId,
                UserId = comment.UserId,
                AuthorName = comment.User?.FirstName + " " + comment.User?.LastName ?? "Anonymous",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                ParentCommentId = comment.ParentCommentId
            };
        }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public int UserId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }
    }
}