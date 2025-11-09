using System.Collections.Generic;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Models.ViewModels
{
    public class BlogIndexViewModel
    {
        public List<BlogPostDto> Posts { get; set; } = new List<BlogPostDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchQuery { get; set; }
        public List<string> SelectedTags { get; set; } = new List<string>();
        public Dictionary<string, int> TagCounts { get; set; } = new Dictionary<string, int>();
    }
}