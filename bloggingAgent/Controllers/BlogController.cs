using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.SEO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<ContentAnalytics> _analyticsRepository;
        private readonly IBloggingAgent _bloggingAgent;
        private readonly ISeoService _seoService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(
            IRepository<BlogPost> blogPostRepository,
            IRepository<ContentAnalytics> analyticsRepository,
            IBloggingAgent bloggingAgent,
            ISeoService seoService,
            ICacheService cacheService,
            ILogger<BlogController> logger)
        {
            _blogPostRepository = blogPostRepository;
            _analyticsRepository = analyticsRepository;
            _bloggingAgent = bloggingAgent;
            _seoService = seoService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchQuery, string tag, int page = 1)
        {
            const int pageSize = 10;
            var cacheKey = $"blog_index_{searchQuery}_{tag}_{page}";

            // Try cache first
            var cachedModel = await _cacheService.GetAsync<BlogIndexViewModel>(cacheKey);
            if (cachedModel != null)
                return View(cachedModel);

            var query = await _blogPostRepository.GetAllAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                        p.Content.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(p => p.Tags.Contains(tag));
            }

            // Get published posts only
            query = query.Where(p => p.IsPublished);

            // Order by creation date
            var posts = query.OrderByDescending(p => p.CreatedAt)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .Select(MapToDto)
                           .ToList();

            // Get tag counts
            var allPosts = await _blogPostRepository.GetAllAsync();
            var tagCounts = allPosts.Where(p => p.IsPublished)
                                  .SelectMany(p => p.Tags)
                                  .GroupBy(t => t)
                                  .ToDictionary(g => g.Key, g => g.Count());

            var model = new BlogIndexViewModel
            {
                Posts = posts,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize),
                SearchQuery = searchQuery,
                SelectedTags = string.IsNullOrEmpty(tag) ? new List<string>() : new List<string> { tag },
                TagCounts = tagCounts
            };

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(5));

            return View(model);
        }

        public async Task<IActionResult> Details(string slug)
        {
            var cacheKey = $"blog_details_{slug}";
            var cachedModel = await _cacheService.GetAsync<BlogDetailViewModel>(cacheKey);
            if (cachedModel != null)
                return View(cachedModel);

            var post = (await _blogPostRepository.GetAllAsync())
                      .FirstOrDefault(p => p.Slug == slug && p.IsPublished);

            if (post == null)
                return NotFound();

            // Update analytics
            await UpdatePostAnalyticsAsync(post.Id);

            // Get related posts
            var relatedPosts = await GetRelatedPostsAsync(post);

            var model = new BlogDetailViewModel
            {
                Post = MapToDto(post),
                SeoAnalysis = await _seoService.AnalyzeContentAsync(post.Content, post.Title),
                RelatedPosts = relatedPosts.Select(MapToDto).ToList(),
                CanEdit = true // TODO: Add authorization check
            };

            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(10));

            return View(model);
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Generate(GeneratePostRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            try
            {
                var generatedPost = await _bloggingAgent.GeneratePostAsync(request);

                // Save to database
                var domainPost = MapToDomain(generatedPost);
                await _blogPostRepository.AddAsync(domainPost);

                // Clear cache
                await _cacheService.RemoveAsync("blog_index_*");

                _logger.LogInformation("Generated and saved new blog post: {Title}", generatedPost.Title);

                return RedirectToAction("Details", new { slug = generatedPost.Slug });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating blog post");
                ModelState.AddModelError("", "Error generating blog post. Please try again.");
                return View(request);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            post.IsPublished = true;
            post.UpdatedAt = DateTime.UtcNow;
            await _blogPostRepository.UpdateAsync(post);

            // Clear cache
            await _cacheService.RemoveAsync($"blog_details_{post.Slug}");
            await _cacheService.RemoveAsync("blog_index_*");

            return RedirectToAction("Details", new { slug = post.Slug });
        }

        [HttpPost]
        public async Task<IActionResult> Unpublish(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            post.IsPublished = false;
            post.UpdatedAt = DateTime.UtcNow;
            await _blogPostRepository.UpdateAsync(post);

            // Clear cache
            await _cacheService.RemoveAsync($"blog_details_{post.Slug}");
            await _cacheService.RemoveAsync("blog_index_*");

            return RedirectToAction("Index");
        }

        private async Task UpdatePostAnalyticsAsync(int postId)
        {
            var analytics = await _analyticsRepository.GetByIdAsync(postId);
            if (analytics == null)
            {
                analytics = new ContentAnalytics
                {
                    Id = postId,
                    BlogPostId = postId,
                    LastUpdated = DateTime.UtcNow
                };
                await _analyticsRepository.AddAsync(analytics);
            }

            analytics.Views++;
            analytics.LastUpdated = DateTime.UtcNow;
            await _analyticsRepository.UpdateAsync(analytics);
        }

        private async Task<List<BlogPost>> GetRelatedPostsAsync(BlogPost post, int count = 3)
        {
            var allPosts = await _blogPostRepository.GetAllAsync();
            return allPosts.Where(p => p.Id != post.Id && p.IsPublished)
                          .Where(p => p.Tags.Intersect(post.Tags).Any())
                          .OrderByDescending(p => p.CreatedAt)
                          .Take(count)
                          .ToList();
        }

        private BlogPostDto MapToDto(BlogPost post)
        {
            return new BlogPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                Author = post.Author,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsPublished = post.IsPublished,
                Tags = post.Tags
            };
        }

        private BlogPost MapToDomain(BlogPostDto dto)
        {
            return new BlogPost
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Content = dto.Content,
                Excerpt = dto.Excerpt,
                Author = dto.Author,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                IsPublished = dto.IsPublished,
                Tags = dto.Tags,
                SeoMetadata = new SeoMetadata(), // Will be populated by SEO service
                Analytics = new ContentAnalytics() // Will be populated by analytics service
            };
        }
    }
}