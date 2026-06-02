using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Agents;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.SEO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace BloggingAgent.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<ContentAnalytics> _analyticsRepository;
        private readonly IBloggingAgent _bloggingAgent;
        private readonly ISeoService _seoService;
        private readonly IContentFormatter _contentFormatter;
        private readonly ICacheService _cacheService;
        private readonly ISyndicationService _syndicationService;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogController> _logger;

        public BlogController(
            IRepository<BlogPost> blogPostRepository,
            IRepository<ContentAnalytics> analyticsRepository,
            IBloggingAgent bloggingAgent,
            ISeoService seoService,
            IContentFormatter contentFormatter,
            ICacheService cacheService,
            ISyndicationService syndicationService,
            IMapper mapper,
            ILogger<BlogController> logger)
        {
            _blogPostRepository = blogPostRepository;
            _analyticsRepository = analyticsRepository;
            _bloggingAgent = bloggingAgent;
            _seoService = seoService;
            _contentFormatter = contentFormatter;
            _cacheService = cacheService;
            _syndicationService = syndicationService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("blog")]
        [Route("blog/index")]
        public async Task<IActionResult> Index(string searchQuery, string tag, int page = 1, string sortBy = "date")
        {
            try
            {
                const int pageSize = 12;
                var cacheKey = $"blog_index_{searchQuery}_{tag}_{page}_{sortBy}";

                // Try cache first
                var cachedModel = await _cacheService.GetAsync<BlogIndexViewModel>(cacheKey);
                if (cachedModel != null)
                {
                    _logger.LogDebug("Serving blog index from cache: {CacheKey}", cacheKey);
                    return View(cachedModel);
                }

                var query = await _blogPostRepository.GetAllAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(p =>
                        p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        p.Content.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        p.Tags.Any(t => t.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
                    _logger.LogInformation("Applied search filter: {SearchQuery}", searchQuery);
                }

                if (!string.IsNullOrEmpty(tag))
                {
                    query = query.Where(p => p.Tags.Contains(tag));
                    _logger.LogInformation("Applied tag filter: {Tag}", tag);
                }

                // Get published posts only
                query = query.Where(p => p.IsPublished);

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "title" => query.OrderBy(p => p.Title),
                    "popular" => query.OrderByDescending(p => p.Analytics?.Views ?? 0),
                    "oldest" => query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt) // "date" or default
                };

                var totalPosts = query.Count();
                var posts = query.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .Select(p => _mapper.Map<BlogPostDto>(p))
                               .ToList();

                // Get tag counts for all published posts
                var allPosts = await _blogPostRepository.GetAllAsync();
                var tagCounts = allPosts.Where(p => p.IsPublished)
                                      .SelectMany(p => p.Tags)
                                      .GroupBy(t => t)
                                      .Select(g => new { Tag = g.Key, Count = g.Count() })
                                      .OrderByDescending(x => x.Count)
                                      .Take(20)
                                      .ToDictionary(x => x.Tag, x => x.Count);

                var model = new BlogIndexViewModel
                {
                    Posts = posts,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
                    SearchQuery = searchQuery,
                    SelectedTags = string.IsNullOrEmpty(tag) ? new List<string>() : new List<string> { tag },
                    TagCounts = tagCounts,
                    SortBy = sortBy,
                    TotalPosts = totalPosts
                };

                // Cache for 10 minutes
                await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(10));

                _logger.LogInformation("Generated blog index with {PostCount} posts for page {Page}", posts.Count, page);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while generating blog index.");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = 500,
                    Title = "Blog Unavailable",
                    Message = "We were unable to load the blog at this time.",
                    DetailedMessage = "Please try again later or contact support if the problem continues.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet]
        [Route("blog/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            try
            {
                if (string.IsNullOrEmpty(slug))
                    return NotFound();

                var cacheKey = $"blog_details_{slug}";
                var cachedModel = await _cacheService.GetAsync<BlogDetailViewModel>(cacheKey);
                if (cachedModel != null)
                {
                    _logger.LogDebug("Serving blog details from cache: {Slug}", slug);
                    return View(cachedModel);
                }

                var post = (await _blogPostRepository.GetAllAsync())
                          .FirstOrDefault(p => p.Slug == slug && p.IsPublished);

                if (post == null)
                {
                    _logger.LogWarning("Blog post not found: {Slug}", slug);
                    return NotFound();
                }

                // Update analytics
                await UpdatePostAnalyticsAsync(post.Id);

                // Get related posts
                var relatedPosts = await GetRelatedPostsAsync(post, 4);

                // Generate SEO analysis
                var seoAnalysis = await _seoService.AnalyzeContentAsync(post.Content, post.Title);

                // Extract excerpt if not set
                var excerpt = post.Excerpt;
                if (string.IsNullOrEmpty(excerpt))
                {
                    excerpt = await _contentFormatter.ExtractExcerptAsync(post.Content);
                }

                var model = new BlogDetailViewModel
                {
                    Post = new BlogPostDto
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        Content = post.Content,
                        Excerpt = excerpt,
                        Author = post.Author,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt,
                        IsPublished = post.IsPublished,
                        Tags = post.Tags
                    },
                    SeoAnalysis = seoAnalysis,
                    RelatedPosts = relatedPosts.Select(p => _mapper.Map<BlogPostDto>(p)).ToList(),
                    CanEdit = User.Identity?.IsAuthenticated ?? false // TODO: Add proper authorization
                };

                // Cache for 15 minutes
                await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(15));

                _logger.LogInformation("Served blog post: {Title} ({Slug})", post.Title, slug);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while loading blog post {Slug}.", slug);
                return View("Error", new ErrorViewModel
                {
                    StatusCode = 500,
                    Title = "Unable to Load Post",
                    Message = "We couldn't load the requested blog post right now.",
                    DetailedMessage = "The blog is temporarily unavailable. Please try again later.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet]
        [Route("blog/generate")]
        public IActionResult Generate()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account",
                    new { returnUrl = Url.Action("Generate", "Blog") });
            }

            return View(new GeneratePostRequest());
        }

        [HttpPost]
        [Route("blog/generate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GeneratePostRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "You must be logged in to generate posts.");
                return View(request);
            }

            try
            {
                _logger.LogInformation("Starting blog post generation for topic: {Topic}", request.Topic);

                var generatedPost = await _bloggingAgent.GeneratePostAsync(request);

                // Save to database
                var domainPost = _mapper.Map<BlogPost>(generatedPost);
                await _blogPostRepository.AddAsync(domainPost);

                // Clear relevant caches
                await ClearBlogCachesAsync();

                _logger.LogInformation("Successfully generated and saved blog post: {Title}", generatedPost.Title);

                TempData["Success"] = "Blog post generated successfully!";
                return RedirectToAction("Details", new { slug = generatedPost.Slug });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating blog post for topic: {Topic}", request.Topic);
                ModelState.AddModelError("", $"Error generating blog post: {ex.Message}");
                return View(request);
            }
        }

        [HttpGet]
        [Route("blog/rss")]
        public async Task<IActionResult> Rss()
        {
            try
            {
                var cacheKey = "blog_rss_feed";
                var cachedFeed = await _cacheService.GetAsync<string>(cacheKey);
                if (cachedFeed != null)
                    return Content(cachedFeed, "application/rss+xml");

                var posts = (await _blogPostRepository.GetAllAsync())
                           .Where(p => p.IsPublished)
                           .OrderByDescending(p => p.CreatedAt)
                           .Take(20)
                           .ToList();

                var rssContent = _syndicationService.GenerateRssFeed(posts);

                // Cache for 30 minutes
                await _cacheService.SetAsync(cacheKey, rssContent, TimeSpan.FromMinutes(30));

                return Content(rssContent, "application/rss+xml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating RSS feed.");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = 500,
                    Title = "Feed Unavailable",
                    Message = "The RSS feed cannot be generated right now.",
                    DetailedMessage = "Please try again later or visit the blog directly.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet]
        [Route("blog/sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            try
            {
                var cacheKey = "blog_sitemap";
                var cachedSitemap = await _cacheService.GetAsync<string>(cacheKey);
                if (cachedSitemap != null)
                    return Content(cachedSitemap, "application/xml");

                var posts = (await _blogPostRepository.GetAllAsync())
                           .Where(p => p.IsPublished)
                           .OrderByDescending(p => p.CreatedAt)
                           .ToList();

                var sitemapContent = _syndicationService.GenerateSitemap(posts);

                // Cache for 60 minutes
                await _cacheService.SetAsync(cacheKey, sitemapContent, TimeSpan.FromHours(1));

                return Content(sitemapContent, "application/xml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sitemap.");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = 500,
                    Title = "Site Map Unavailable",
                    Message = "The sitemap cannot be generated at the moment.",
                    DetailedMessage = "Please try again later or contact support for help.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPost]
        [Route("blog/{id}/publish")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(Guid id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            // TODO: Add authorization check
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            post.IsPublished = true;
            post.MarkAsModified();
            await _blogPostRepository.UpdateAsync(post);

            // Clear caches
            await ClearBlogCachesAsync();
            await _cacheService.RemoveAsync($"blog_details_{post.Slug}");

            _logger.LogInformation("Published blog post: {Title}", post.Title);

            TempData["Success"] = "Post published successfully!";
            return RedirectToAction("Details", new { slug = post.Slug });
        }

        [HttpPost]
        [Route("blog/{id}/unpublish")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(Guid id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            // TODO: Add authorization check
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            post.IsPublished = false;
            post.MarkAsModified();
            await _blogPostRepository.UpdateAsync(post);

            // Clear caches
            await ClearBlogCachesAsync();
            await _cacheService.RemoveAsync($"blog_details_{post.Slug}");

            _logger.LogInformation("Unpublished blog post: {Title}", post.Title);

            TempData["Success"] = "Post unpublished successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("blog/tag/{tag}")]
        public async Task<IActionResult> Tag(string tag, int page = 1)
        {
            if (string.IsNullOrEmpty(tag))
                return RedirectToAction("Index");

            return RedirectToAction("Index", new { tag, page });
        }

        [HttpGet]
        [Route("blog/search")]
        public async Task<IActionResult> Search(string q, int page = 1)
        {
            if (string.IsNullOrEmpty(q))
                return RedirectToAction("Index");

            return RedirectToAction("Index", new { searchQuery = q, page });
        }

        // API endpoints for AJAX operations
        [HttpPost]
        [Route("api/blog/{id}/like")]
        public async Task<IActionResult> LikePost(Guid id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            // Update analytics
            var analytics = await _analyticsRepository.GetByIdAsync(id);
            if (analytics == null)
            {
                analytics = new ContentAnalytics
                {
                    BlogPostId = id
                };
                await _analyticsRepository.AddAsync(analytics);
            }

            analytics.Comments++; // Using comments as proxy for engagement
            await _analyticsRepository.UpdateAsync(analytics);

            return Json(new { success = true, likes = analytics.Comments });
        }

        private async Task UpdatePostAnalyticsAsync(Guid postId)
        {
            var analytics = await _analyticsRepository.GetByIdAsync(postId);
            if (analytics == null)
            {
                analytics = new ContentAnalytics
                {
                    BlogPostId = postId
                };
                await _analyticsRepository.AddAsync(analytics);
            }

            analytics.Views++;
            await _analyticsRepository.UpdateAsync(analytics);

            _logger.LogDebug("Updated analytics for post {PostId}: {Views} views", postId, analytics.Views);
        }

        private async Task<List<BlogPost>> GetRelatedPostsAsync(BlogPost post, int count = 4)
        {
            var allPosts = await _blogPostRepository.GetAllAsync();
            return allPosts.Where(p => p.Id != post.Id && p.IsPublished)
                          .Where(p => p.Tags.Intersect(post.Tags).Any() ||
                                    p.Title.Contains(post.Title.Split(' ')[0], StringComparison.OrdinalIgnoreCase))
                          .OrderByDescending(p => p.CreatedAt)
                          .Take(count)
                          .ToList();
        }

        private async Task ClearBlogCachesAsync()
        {
            // Clear all blog-related caches
            await _cacheService.RemoveAsync("blog_index_*");
            await _cacheService.RemoveAsync("blog_details_*");
            await _cacheService.RemoveAsync("blog_rss_feed");
            await _cacheService.RemoveAsync("blog_sitemap");

            _logger.LogDebug("Cleared all blog-related caches");
        }

        

        
    }
}