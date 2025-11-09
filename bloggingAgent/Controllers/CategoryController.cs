using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            IRepository<Category> categoryRepository,
            IRepository<BlogPost> blogPostRepository,
            ILogger<CategoryController> logger)
        {
            _categoryRepository = categoryRepository;
            _blogPostRepository = blogPostRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var activeCategories = categories.Where(c => c.IsActive)
                                           .OrderBy(c => c.DisplayOrder)
                                           .ToList();

            return View(activeCategories);
        }

        public async Task<IActionResult> Details(string slug)
        {
            var category = (await _categoryRepository.GetAllAsync())
                          .FirstOrDefault(c => c.Slug == slug && c.IsActive);

            if (category == null)
            {
                return NotFound();
            }

            // Get posts in this category
            var posts = await _blogPostRepository.FindAsync(p =>
                p.IsPublished &&
                p.Tags.Any(tag => tag.Contains(category.Name.ToLower()))); // Simple tag-based categorization

            var viewModel = new CategoryDetailViewModel
            {
                Category = category,
                Posts = posts.OrderByDescending(p => p.CreatedAt).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Generate slug if not provided
            if (string.IsNullOrEmpty(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
            }

            category.IsActive = true;
            category.DisplayOrder = await GetNextDisplayOrderAsync();

            await _categoryRepository.AddAsync(category);

            _logger.LogInformation("Category created: {CategoryName}", category.Name);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;
            existingCategory.Slug = string.IsNullOrEmpty(category.Slug) ?
                                   GenerateSlug(category.Name) : category.Slug;
            existingCategory.Description = category.Description;
            existingCategory.ParentCategoryId = category.ParentCategoryId;
            existingCategory.DisplayOrder = category.DisplayOrder;

            await _categoryRepository.UpdateAsync(existingCategory);

            _logger.LogInformation("Category updated: {CategoryName}", category.Name);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Soft delete - mark as inactive
            category.IsActive = false;
            await _categoryRepository.UpdateAsync(category);

            _logger.LogInformation("Category deactivated: {CategoryName}", category.Name);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reorder(List<CategoryOrder> categoryOrders)
        {
            foreach (var order in categoryOrders)
            {
                var category = await _categoryRepository.GetByIdAsync(order.Id);
                if (category != null)
                {
                    category.DisplayOrder = order.Order;
                    await _categoryRepository.UpdateAsync(category);
                }
            }

            return Json(new { success = true });
        }

        private async Task<int> GetNextDisplayOrderAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Any() ? categories.Max(c => c.DisplayOrder) + 1 : 1;
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

    public class CategoryDetailViewModel
    {
        public Category Category { get; set; }
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
    }

    public class CategoryOrder
    {
        public int Id { get; set; }
        public int Order { get; set; }
    }
}