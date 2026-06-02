using BloggingAgent.Data.EntityConfigurations;
using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AgentMemory> AgentMemories { get; set; }
        public DbSet<SeoMetadata> SeoMetadata { get; set; }
        public DbSet<ContentAnalytics> ContentAnalytics { get; set; }
        public DbSet<AgentSettings> AgentSettings { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new AgentMemoryConfiguration());
            modelBuilder.ApplyConfiguration(new SeoMetadataConfiguration());
            modelBuilder.ApplyConfiguration(new ContentAnalyticsConfiguration());
            modelBuilder.ApplyConfiguration(new AgentSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
        }
    }
}