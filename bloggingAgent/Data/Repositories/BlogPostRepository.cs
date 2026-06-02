using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Data.Repositories
{
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page = 1, int pageSize = 10)
        {
            return await _dbSet
                .Where(p => p.IsPublished)
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tag, int page = 1, int pageSize = 10)
        {
            return await _dbSet
                .Where(p => p.IsPublished && p.Tags.Contains(tag))
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> SearchPostsAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            var searchTermLower = searchTerm.ToLower();
            return await _dbSet
                .Where(p => p.IsPublished &&
                           (p.Title.ToLower().Contains(searchTermLower) ||
                            p.Content.ToLower().Contains(searchTermLower) ||
                            p.Excerpt.ToLower().Contains(searchTermLower) ||
                            p.Tags.Any(t => t.ToLower().Contains(searchTermLower))))
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<BlogPost> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
        }

        public async Task<Dictionary<string, int>> GetTagCountsAsync()
        {
            var posts = await _dbSet
                .Where(p => p.IsPublished)
                .ToListAsync();

            return posts.SelectMany(p => p.Tags)
                       .GroupBy(t => t)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<int> GetPublishedPostCountAsync()
        {
            return await _dbSet.CountAsync(p => p.IsPublished);
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 5)
        {
            return await _dbSet
                .Where(p => p.IsPublished)
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public override async Task<BlogPost> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .FirstOrDefaultAsync(p => p.Id == (Guid)id);
        }

        public override async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.SeoMetadata)
                .Include(p => p.Analytics)
                .ToListAsync();
        }
    }
}