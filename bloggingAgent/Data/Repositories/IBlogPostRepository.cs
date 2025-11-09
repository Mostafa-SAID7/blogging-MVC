using System.Collections.Generic;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Data.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page = 1, int pageSize = 10);
        Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tag, int page = 1, int pageSize = 10);
        Task<IEnumerable<BlogPost>> SearchPostsAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<BlogPost> GetBySlugAsync(string slug);
        Task<Dictionary<string, int>> GetTagCountsAsync();
        Task<int> GetPublishedPostCountAsync();
        Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 5);
    }
}