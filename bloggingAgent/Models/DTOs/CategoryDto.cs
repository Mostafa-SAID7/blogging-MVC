using System.Collections.Generic;

namespace BloggingAgent.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public int PostCount { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public int? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public int? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CategoryDetailViewModel
    {
        public CategoryDto Category { get; set; }
        public List<BlogPostDto> Posts { get; set; } = new List<BlogPostDto>();
        public int TotalPosts { get; set; }
    }
}