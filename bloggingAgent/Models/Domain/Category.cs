using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>();
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}