using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly ICacheService _cacheService;

        public CategoryController(
            IRepository<Category> categoryRepository,
            IRepository<BlogPost> blogPostRepository,
            ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _blogPostRepository = blogPostRepository;
            _cacheService = cacheService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "categories_index";
            var cachedCategories = await _cacheService.GetAsync<List<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return View(cachedCategories);
            }

            var categories = await _categoryRepository.GetAllAsync();
            var activeCategories = categories.Where(c => c.IsActive)
                                           .OrderBy(c => c.DisplayOrder)
                                           .ThenBy(c => c.Name)
                                           .ToList();

            // Calculate post counts
            foreach (var category in activeCategories)
            {
                category.PostCount = await GetPostCountForCategoryAsync(category.Id);
            }

            var categoryDtos = activeCategories.Select(MapToDto).ToList();

            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, categoryDtos, TimeSpan.FromMinutes(10));

            return View(categoryDtos);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string slug)
        {
            var cacheKey = $"category_details_{slug}";
            var cachedModel = await _cacheService.GetAsync<CategoryDetailViewModel>(cacheKey);
            if (cachedModel != null)
            {
                return View(cachedModel);
            }

            var category = (await _categoryRepository.GetAllAsync())
                          .FirstOrDefault(c => c.Slug == slug && c.IsActive);

            if (category == null)
            {
                return NotFound();
            }

            // Get posts in this category
            var posts = await _blogPostRepository.FindAsync(p =>
                p.Tags.Contains(category.Name) && p.IsPublished);

            var model = new CategoryDetailViewModel
            {
                Category = MapToDto(category),
                Posts = posts.OrderByDescending(p => p.CreatedAt)
                           .Select(p => new BlogPostDto
                           {
                               Id = p.Id,
                               Title = p.Title,
                               Slug = p.Slug,
                               Excerpt = p.Excerpt,
                               Author = p.Author,
                               CreatedAt = p.CreatedAt,
                               Tags = p.Tags
                           }).ToList(),
                TotalPosts = posts.Count()
            };

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, model, TimeSpan.FromMinutes(5));

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Manage()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = categories.OrderBy(c => c.DisplayOrder)
                                       .ThenBy(c => c.Name)
                                       .Select(MapToDto)
                                       .ToList();

            return View(categoryDtos);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View(new CreateCategoryRequest());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            // Check if slug is unique
            var existingCategory = (await _categoryRepository.GetAllAsync())
                                  .FirstOrDefault(c => c.Slug == GenerateSlug(request.Name));

            if (existingCategory != null)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View(request);
            }

            var category = new Category
            {
                Name = request.Name,
                Slug = GenerateSlug(request.Name),
                Description = request.Description,
                Color = request.Color ?? "#007bff",
                Icon = request.Icon ?? "fas fa-tag",
                ParentCategoryId = request.ParentCategoryId,
                DisplayOrder = request.DisplayOrder,
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);

            // Clear cache
            await _cacheService.RemoveAsync("categories_index");

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new UpdateCategoryRequest
            {
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                Icon = category.Icon,
                ParentCategoryId = category.ParentCategoryId,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check slug uniqueness if name changed
            if (category.Name != request.Name)
            {
                var newSlug = GenerateSlug(request.Name);
                var existingCategory = (await _categoryRepository.GetAllAsync())
                                      .FirstOrDefault(c => c.Slug == newSlug && c.Id != id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(request);
                }

                category.Slug = newSlug;
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.Color = request.Color;
            category.Icon = request.Icon;
            category.ParentCategoryId = request.ParentCategoryId;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;
            category.UpdatedAt = System.DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            // Clear cache
            await _cacheService.RemoveAsync("categories_index");
            await _cacheService.RemoveAsync($"category_details_{category.Slug}");

            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category has posts
            var postCount = await GetPostCountForCategoryAsync(id);
            if (postCount > 0)
            {
                TempData["Error"] = "Cannot delete category that contains posts. Please reassign or delete the posts first.";
                return RedirectToAction("Manage");
            }

            await _categoryRepository.DeleteAsync(id);

            // Clear cache
            await _cacheService.RemoveAsync("categories_index");

            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.IsActive = !category.IsActive;
            category.UpdatedAt = System.DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            // Clear cache
            await _cacheService.RemoveAsync("categories_index");
            await _cacheService.RemoveAsync($"category_details_{category.Slug}");

            return Json(new { success = true, isActive = category.IsActive });
        }

        private async Task<int> GetPostCountForCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) return 0;

            var posts = await _blogPostRepository.GetAllAsync();
            return posts.Count(p => p.Tags.Contains(category.Name) && p.IsPublished);
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                Color = category.Color,
                Icon = category.Icon,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive,
                DisplayOrder = category.DisplayOrder,
                PostCount = category.PostCount
            };
        }

        private string GenerateSlug(string name)
        {
            return name.ToLower()
                      .Replace(" ", "-")
                      .Replace(".", "")
                      .Replace(",", "")
                      .Replace("?", "")
                      .Replace("!", "")
                      .Replace(":", "")
                      .Replace(";", "");
        }
    }

}