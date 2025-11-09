using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<AgentMemory> AgentMemories { get; set; }
        public DbSet<SeoMetadata> SeoMetadata { get; set; }
        public DbSet<ContentAnalytics> ContentAnalytics { get; set; }
        public DbSet<AgentSettings> AgentSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BlogPost configuration
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Author).HasMaxLength(100);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Excerpt).HasMaxLength(500);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsPublished);

                // Configure Tags as JSON
                entity.Property(e => e.Tags)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                // Configure relationships
                entity.HasOne(e => e.SeoMetadata)
                      .WithOne()
                      .HasForeignKey<BlogPost>(e => e.Id)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Analytics)
                      .WithOne()
                      .HasForeignKey<BlogPost>(e => e.Id)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AgentMemory configuration
            modelBuilder.Entity<AgentMemory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.HasIndex(e => e.Key);
                entity.HasIndex(e => new { e.Key, e.Category });
                entity.HasIndex(e => e.ExpiresAt);
            });

            // SeoMetadata configuration
            modelBuilder.Entity<SeoMetadata>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(60);
                entity.Property(e => e.Description).HasMaxLength(160);
                entity.Property(e => e.Keywords).HasMaxLength(255);
                entity.Property(e => e.CanonicalUrl).HasMaxLength(500);
                entity.Property(e => e.OgTitle).HasMaxLength(60);
                entity.Property(e => e.OgDescription).HasMaxLength(160);
                entity.Property(e => e.OgImage).HasMaxLength(500);
                entity.Property(e => e.TwitterCard).HasMaxLength(50);

                // Configure StructuredData as JSON
                entity.Property(e => e.StructuredData)
                      .HasConversion(
                          v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                          v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, string>()
                      );
            });

            // ContentAnalytics configuration
            modelBuilder.Entity<ContentAnalytics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BlogPostId).IsRequired();
                entity.Property(e => e.AverageReadTime).HasColumnType("decimal(5,2)");
                entity.Property(e => e.BounceRate).HasColumnType("decimal(5,2)");
                entity.HasIndex(e => e.BlogPostId).IsUnique();
                entity.HasIndex(e => e.LastUpdated);

                // Configure TrafficSources as JSON
                entity.Property(e => e.TrafficSources)
                      .HasConversion(
                          v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                          v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, int>()
                      );
            });

            // AgentSettings configuration
            modelBuilder.Entity<AgentSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DefaultAuthor).HasMaxLength(100);
                entity.Property(e => e.Theme).HasMaxLength(50);

                // Configure DefaultTags as JSON
                entity.Property(e => e.DefaultTags)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                // Configure CustomSettings as JSON
                entity.Property(e => e.CustomSettings)
                      .HasConversion(
                          v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                          v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, object>()
                      );
            });
        }
    }
}