using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.SEO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IBloggingAgent _bloggingAgent;
        private readonly ISeoService _seoService;
        private readonly ICacheService _cacheService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BlogController> _logger;

        public BlogController(
            IRepository<BlogPost> blogPostRepository,
            IBloggingAgent bloggingAgent,
            ISeoService seoService,
            ICacheService cacheService,
            UserManager<ApplicationUser> userManager,
            ILogger<BlogController> logger)
        {
            _blogPostRepository = blogPostRepository;
            _bloggingAgent = bloggingAgent;
            _seoService = seoService;
            _cacheService = cacheService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null, [FromQuery] string tag = null)
        {
            var cacheKey = $"api_posts_{page}_{pageSize}_{search}_{tag}";
            var cached = await _cacheService.GetAsync<object>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            var query = await _blogPostRepository.GetAllAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                        p.Content.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(p => p.Tags.Contains(tag));
            }

            // Only published posts for anonymous users
            if (!User.Identity.IsAuthenticated)
            {
                query = query.Where(p => p.IsPublished);
            }

            var totalCount = query.Count();
            var posts = query.OrderByDescending(p => p.CreatedAt)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .Select(MapToApiResponse)
                           .ToList();

            var result = new
            {
                posts = posts,
                pagination = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            };

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(int id)
        {
            var cacheKey = $"api_post_{id}";
            var cached = await _cacheService.GetAsync<BlogPostApiResponse>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            // Check if user can view unpublished posts
            if (!post.IsPublished && !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var response = MapToApiResponse(post);
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Author,Editor,Administrator")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var generateRequest = new GeneratePostRequest
            {
                Topic = request.Title,
                Keywords = string.Join(",", request.Tags),
                TargetWordCount = request.TargetWordCount,
                Tags = request.Tags,
                Tone = request.Tone,
                TargetAudience = request.TargetAudience,
                IncludeImages = request.IncludeImages
            };

            try
            {
                var generatedPost = await _bloggingAgent.GeneratePostAsync(generateRequest);

                // Create domain post
                var post = new BlogPost
                {
                    Title = generatedPost.Title,
                    Slug = generatedPost.Slug,
                    Content = generatedPost.Content,
                    Excerpt = generatedPost.Excerpt,
                    AuthorId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsPublished = request.PublishImmediately,
                    Tags = generatedPost.Tags
                };

                await _blogPostRepository.AddAsync(post);

                // Clear cache
                await _cacheService.RemoveAsync("api_posts_*");

                _logger.LogInformation("Post created by user {UserId}: {Title}", user.Id, post.Title);

                return CreatedAtAction(nameof(GetPost), new { id = post.Id }, MapToApiResponse(post));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post for user {UserId}", user.Id);
                return BadRequest(new { error = "Failed to create post. Please try again." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Author,Editor,Administrator")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            // Check ownership or admin rights
            var user = await _userManager.GetUserAsync(User);
            var isOwner = post.AuthorId == user.Id;
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            var isEditor = await _userManager.IsInRoleAsync(user, "Editor");

            if (!isOwner && !isAdmin && !isEditor)
            {
                return Forbid();
            }

            post.Title = request.Title ?? post.Title;
            post.Content = request.Content ?? post.Content;
            post.Excerpt = request.Excerpt ?? post.Excerpt;
            post.Tags = request.Tags ?? post.Tags;
            post.IsPublished = request.IsPublished ?? post.IsPublished;
            post.UpdatedAt = DateTime.UtcNow;

            await _blogPostRepository.UpdateAsync(post);

            // Clear cache
            await _cacheService.RemoveAsync($"api_post_{id}");
            await _cacheService.RemoveAsync("api_posts_*");

            _logger.LogInformation("Post {PostId} updated by user {UserId}", id, user.Id);

            return Ok(MapToApiResponse(post));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            await _blogPostRepository.DeleteAsync(id);

            // Clear cache
            await _cacheService.RemoveAsync($"api_post_{id}");
            await _cacheService.RemoveAsync("api_posts_*");

            var user = await _userManager.GetUserAsync(User);
            _logger.LogInformation("Post {PostId} deleted by user {UserId}", id, user.Id);

            return NoContent();
        }

        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Editor,Administrator")]
        public async Task<IActionResult> PublishPost(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            post.IsPublished = true;
            post.UpdatedAt = DateTime.UtcNow;
            await _blogPostRepository.UpdateAsync(post);

            // Clear cache
            await _cacheService.RemoveAsync($"api_post_{id}");
            await _cacheService.RemoveAsync("api_posts_*");

            var user = await _userManager.GetUserAsync(User);
            _logger.LogInformation("Post {PostId} published by user {UserId}", id, user.Id);

            return Ok(new { message = "Post published successfully" });
        }

        [HttpPost("{id}/seo-analyze")]
        [Authorize(Roles = "Author,Editor,Administrator")]
        public async Task<IActionResult> AnalyzeSeo(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            // Check ownership or admin rights
            var user = await _userManager.GetUserAsync(User);
            var isOwner = post.AuthorId == user.Id;
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            var isEditor = await _userManager.IsInRoleAsync(user, "Editor");

            if (!isOwner && !isAdmin && !isEditor)
            {
                return Forbid();
            }

            var analysis = await _seoService.AnalyzeContentAsync(post.Content, post.Title);

            return Ok(new
            {
                postId = id,
                analysis = analysis,
                suggestions = analysis.Suggestions,
                score = analysis.Score
            });
        }

        private BlogPostApiResponse MapToApiResponse(BlogPost post)
        {
            return new BlogPostApiResponse
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                Author = new
                {
                    id = post.Author?.Id,
                    name = post.Author?.DisplayName,
                    email = post.Author?.Email
                },
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsPublished = post.IsPublished,
                Tags = post.Tags,
                CommentCount = post.Comments?.Count(c => c.IsApproved && !c.IsSpam) ?? 0
            };
        }
    }

    // API Request/Response Models
    public class BlogPostApiResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public object Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; }
        public int CommentCount { get; set; }
    }

    public class CreatePostRequest
    {
        public string Title { get; set; }
        public int TargetWordCount { get; set; } = 500;
        public List<string> Tags { get; set; } = new List<string>();
        public string Tone { get; set; }
        public string TargetAudience { get; set; }
        public bool IncludeImages { get; set; }
        public bool PublishImmediately { get; set; }
    }

    public class UpdatePostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public List<string> Tags { get; set; }
        public bool? IsPublished { get; set; }
    }
}