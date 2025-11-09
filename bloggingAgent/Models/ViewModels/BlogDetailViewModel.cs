using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Models.ViewModels
{
    public class BlogDetailViewModel
    {
        public BlogPostDto Post { get; set; }
        public SeoAnalysisResult SeoAnalysis { get; set; }
        public List<BlogPostDto> RelatedPosts { get; set; } = new List<BlogPostDto>();
        public bool CanEdit { get; set; }
    }
}