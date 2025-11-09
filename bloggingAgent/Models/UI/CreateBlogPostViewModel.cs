using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BloggingAgent.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace BloggingAgent.Models.UI
{
    public class CreateBlogPostViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        [Display(Name = "Post Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters long")]
        [Display(Name = "Post Content")]
        public string Content { get; set; }

        [StringLength(500, ErrorMessage = "Excerpt cannot exceed 500 characters")]
        [Display(Name = "Excerpt (optional)")]
        public string Excerpt { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public BlogCategory Category { get; set; }

        [Required(ErrorMessage = "Tone is required")]
        [Display(Name = "Content Tone")]
        public ContentTone Tone { get; set; } = ContentTone.Professional;

        [Display(Name = "Tags (comma-separated)")]
        [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
        public string TagsString { get; set; }

        [Display(Name = "Featured Image")]
        public IFormFile FeaturedImage { get; set; }

        [Display(Name = "Publish immediately")]
        public bool PublishImmediately { get; set; } = false;

        [Display(Name = "Schedule for later")]
        public bool IsScheduled { get; set; } = false;

        [Display(Name = "Publish Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledDate { get; set; }

        [Display(Name = "SEO Title")]
        [StringLength(60, ErrorMessage = "SEO title cannot exceed 60 characters")]
        public string SeoTitle { get; set; }

        [Display(Name = "Meta Description")]
        [StringLength(160, ErrorMessage = "Meta description cannot exceed 160 characters")]
        public string MetaDescription { get; set; }

        [Display(Name = "Focus Keywords")]
        [StringLength(200, ErrorMessage = "Focus keywords cannot exceed 200 characters")]
        public string FocusKeywords { get; set; }

        // Helper properties
        public List<string> AvailableTags { get; set; } = new List<string>();
        public bool IsEditMode { get; set; } = false;
        public int? ExistingPostId { get; set; }

        // Validation
        public bool IsValidScheduledDate()
        {
            if (!IsScheduled) return true;
            return ScheduledDate.HasValue && ScheduledDate.Value > DateTime.Now;
        }

        public List<string> GetTagsList()
        {
            if (string.IsNullOrWhiteSpace(TagsString))
                return new List<string>();

            return TagsString.Split(',')
                           .Select(t => t.Trim().ToLower())
                           .Where(t => !string.IsNullOrWhiteSpace(t))
                           .Distinct()
                           .ToList();
        }

        public void SetTagsFromList(List<string> tags)
        {
            TagsString = string.Join(", ", tags);
        }
    }

    public class EditBlogPostViewModel : CreateBlogPostViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PostStatus CurrentStatus { get; set; }
        public string CurrentSlug { get; set; }

        public EditBlogPostViewModel()
        {
            IsEditMode = true;
        }
    }

    public class BlogPostPreviewViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public BlogCategory Category { get; set; }
        public ContentTone Tone { get; set; }
        public List<string> Tags { get; set; }
        public string SeoTitle { get; set; }
        public string MetaDescription { get; set; }
        public int EstimatedReadingTime { get; set; }
        public int WordCount { get; set; }
    }
}