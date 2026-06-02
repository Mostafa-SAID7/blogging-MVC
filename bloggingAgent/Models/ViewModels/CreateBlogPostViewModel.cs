using System.ComponentModel.DataAnnotations;

namespace BloggingAgent.Models.ViewModels
{
    public class CreateBlogPostViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; }

        [StringLength(200)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string Excerpt { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(50, ErrorMessage = "Content must be at least 50 characters")]
        public string Content { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(50)]
        public string Tone { get; set; }

        [StringLength(500)]
        public string Tags { get; set; }

        [StringLength(160)]
        public string SeoTitle { get; set; }

        [StringLength(160)]
        public string SeoDescription { get; set; }

        [StringLength(200)]
        public string SeoKeywords { get; set; }

        public string Status { get; set; } = "Draft";
    }
}
