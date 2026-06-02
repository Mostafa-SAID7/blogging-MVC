using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    [Authorize]
    public class SocialMediaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<SocialMediaAccount> _accountRepository;
        private readonly IRepository<SocialMediaPost> _postRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly ICacheService _cacheService;

        public SocialMediaController(
            UserManager<ApplicationUser> userManager,
            IRepository<SocialMediaAccount> accountRepository,
            IRepository<SocialMediaPost> postRepository,
            IRepository<BlogPost> blogPostRepository,
            ICacheService cacheService)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            _postRepository = postRepository;
            _blogPostRepository = blogPostRepository;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cacheKey = $"social_accounts_{user.Id}";

            var cachedAccounts = await _cacheService.GetAsync<List<SocialMediaAccount>>(cacheKey);
            if (cachedAccounts != null)
            {
                return View(cachedAccounts);
            }

            var accounts = await _accountRepository.FindAsync(a => a.UserId == user.Id && a.IsActive);
            var accountList = accounts.ToList();

            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, accountList, TimeSpan.FromMinutes(10));

            return View(accountList);
        }

        [HttpGet]
        public IActionResult Connect()
        {
            var supportedPlatforms = new List<SocialPlatform>
            {
                new SocialPlatform { Name = "Twitter", Icon = "fab fa-twitter", Color = "#1DA1F2", IsAvailable = true },
                new SocialPlatform { Name = "LinkedIn", Icon = "fab fa-linkedin", Color = "#0077B5", IsAvailable = true },
                new SocialPlatform { Name = "Facebook", Icon = "fab fa-facebook", Color = "#1877F2", IsAvailable = true },
                new SocialPlatform { Name = "Instagram", Icon = "fab fa-instagram", Color = "#E4405F", IsAvailable = true },
                new SocialPlatform { Name = "YouTube", Icon = "fab fa-youtube", Color = "#FF0000", IsAvailable = false },
                new SocialPlatform { Name = "TikTok", Icon = "fab fa-tiktok", Color = "#000000", IsAvailable = false }
            };

            return View(supportedPlatforms);
        }

        [HttpPost]
        public async Task<IActionResult> ConnectAccount(string platform)
        {
            var user = await _userManager.GetUserAsync(User);

            // Check if account already exists
            var existingAccount = await _accountRepository.SingleOrDefaultAsync(a =>
                a.UserId == user.Id && a.Platform == platform && a.IsActive);

            if (existingAccount != null)
            {
                TempData["Error"] = $"{platform} account is already connected.";
                return RedirectToAction("Index");
            }

            // In a real implementation, this would redirect to OAuth flow
            // For now, we'll simulate the connection process
            var account = new SocialMediaAccount
            {
                UserId = user.Id,
                Platform = platform.ToLower(),
                PlatformUsername = $"{user.UserName}_{platform.ToLower()}",
                PlatformDisplayName = user.UserName,
                IsActive = true,
                AccessToken = "simulated_token", // Would be real token from OAuth
                TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _accountRepository.AddAsync(account);

            // Clear cache
            await _cacheService.RemoveAsync($"social_accounts_{user.Id}");

            TempData["Success"] = $"{platform} account connected successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DisconnectAccount(int accountId)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null || account.UserId != user.Id)
            {
                return NotFound();
            }

            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);

            // Clear cache
            await _cacheService.RemoveAsync($"social_accounts_{user.Id}");

            TempData["Success"] = $"{account.PlatformDisplayName} account disconnected.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Posts(int accountId = 0)
        {
            var user = await _userManager.GetUserAsync(User);

            List<SocialMediaPost> posts;
            if (accountId > 0)
            {
                // Get posts for specific account
                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null || account.UserId != user.Id)
                {
                    return NotFound();
                }
                posts = (await _postRepository.FindAsync(p => p.SocialMediaAccountId == accountId)).ToList();
            }
            else
            {
                // Get posts for all user's accounts
                var accounts = await _accountRepository.FindAsync(a => a.UserId == user.Id && a.IsActive);
                var accountIds = accounts.Select(a => a.Id).ToList();
                posts = (await _postRepository.FindAsync(p => accountIds.Contains(p.SocialMediaAccountId))).ToList();
            }

            var model = new SocialPostsViewModel
            {
                Posts = posts.OrderByDescending(p => p.CreatedAt).ToList(),
                Accounts = (await _accountRepository.FindAsync(a => a.UserId == user.Id && a.IsActive)).ToList(),
                SelectedAccountId = accountId
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePost(int? blogPostId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var accounts = await _accountRepository.FindAsync(a => a.UserId == user.Id && a.IsActive);

            var model = new CreateSocialPostViewModel
            {
                AvailableAccounts = accounts.ToList(),
                BlogPostId = blogPostId
            };

            if (blogPostId.HasValue)
            {
                var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId.Value);
                if (blogPost != null && blogPost.Author == user.UserName)
                {
                    model.Title = blogPost.Title;
                    model.Content = blogPost.Excerpt ?? blogPost.Content?.Substring(0, Math.Min(200, blogPost.Content.Length));
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreateSocialPostViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                model.AvailableAccounts = (await _accountRepository.FindAsync(a => a.UserId == user.Id && a.IsActive)).ToList();
                return View(model);
            }

            foreach (var accountId in model.SelectedAccountIds)
            {
                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null || account.UserId != user.Id)
                {
                    continue;
                }

                var post = new SocialMediaPost
                {
                    SocialMediaAccountId = accountId,
                    BlogPostId = model.BlogPostId,
                    Platform = account.Platform,
                    Content = model.Content,
                    Title = model.Title,
                    Status = model.IsScheduled ? "scheduled" : "draft",
                    ScheduledAt = model.ScheduledAt,
                    Tags = string.Join(",", model.Tags),
                    Campaign = model.Campaign,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _postRepository.AddAsync(post);
            }

            TempData["Success"] = "Social media posts created successfully!";
            return RedirectToAction("Posts");
        }

        [HttpPost]
        public async Task<IActionResult> PublishPost(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            var account = await _accountRepository.GetByIdAsync(post.SocialMediaAccountId);
            if (account == null || account.UserId != user.Id)
            {
                return NotFound();
            }

            // In a real implementation, this would post to the actual social media platform
            // For now, we'll simulate the posting process
            post.MarkAsPosted($"simulated_{postId}", DateTime.UtcNow);
            account.IncrementPostCount();

            await _postRepository.UpdateAsync(post);
            await _accountRepository.UpdateAsync(account);

            TempData["Success"] = $"Post published to {account.PlatformDisplayName}!";
            return RedirectToAction("Posts");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            var account = await _accountRepository.GetByIdAsync(post.SocialMediaAccountId);
            if (account == null || account.UserId != user.Id)
            {
                return NotFound();
            }

            await _postRepository.DeleteAsync(postId);

            TempData["Success"] = "Post deleted successfully.";
            return RedirectToAction("Posts");
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            var user = await _userManager.GetUserAsync(User);
            var accounts = await _accountRepository.FindAsync(a => a.UserId == user.Id && a.IsActive);
            var accountIds = accounts.Select(a => a.Id).ToList();

            var posts = await _postRepository.FindAsync(p => accountIds.Contains(p.SocialMediaAccountId));

            var model = new SocialAnalyticsViewModel
            {
                Accounts = accounts.ToList(),
                TotalPosts = posts.Count(),
                PostedPosts = posts.Count(p => p.IsPosted),
                ScheduledPosts = posts.Count(p => p.IsScheduled),
                FailedPosts = posts.Count(p => p.HasFailed),
                TotalEngagement = posts.Sum(p => p.Likes + p.Shares + p.Comments),
                AverageEngagementRate = posts.Any() ? posts.Average(p => p.EngagementRate) : 0,
                PostsByPlatform = posts.GroupBy(p => p.Platform)
                                      .ToDictionary(g => g.Key, g => g.Count()),
                RecentPosts = posts.OrderByDescending(p => p.CreatedAt)
                                  .Take(10)
                                  .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccountSettings(int accountId, bool autoPostEnabled, string postTemplate)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null || account.UserId != user.Id)
            {
                return NotFound();
            }

            account.AutoPostEnabled = autoPostEnabled;
            account.DefaultPostTemplate = postTemplate;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);

            // Clear cache
            await _cacheService.RemoveAsync($"social_accounts_{user.Id}");

            return Json(new { success = true });
        }
    }

}