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
                entity.HasIndex(e => e.Status);

                // Configure Tags as JSON
                entity.Property(e => e.Tags)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                // Relationships
                entity.HasOne(e => e.SeoMetadata)
                      .WithOne()
                      .HasForeignKey<BlogPost>(e => e.Id)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Analytics)
                      .WithOne()
                      .HasForeignKey<BlogPost>(e => e.Id)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<ApplicationUser>()
                      .WithMany(u => u.Posts)
                      .HasForeignKey(e => e.AuthorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Comment configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.BlogPostId, e.IsApproved, e.IsSpam });

                // Self-referencing relationship for nested comments
                entity.HasOne(e => e.ParentComment)
                      .WithMany()
                      .HasForeignKey(e => e.ParentCommentId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Relationships
                entity.HasOne(e => e.BlogPost)
                      .WithMany()
                      .HasForeignKey(e => e.BlogPostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationUser>()
                      .WithMany(u => u.Comments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsActive);

                // Self-referencing relationship for hierarchical categories
                // Using NoAction to prevent circular cascade paths in SQL Server
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.SubCategories)
                      .HasForeignKey(e => e.ParentCategoryId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // AgentMemory configuration
            modelBuilder.Entity<AgentMemory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Metadata)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new Dictionary<string, object>())
                    .HasColumnType("TEXT");
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

            // UserLogin configuration
            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProviderKey).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ProviderDisplayName).HasMaxLength(200);
                entity.HasIndex(e => new { e.UserId, e.Provider });

                entity.HasOne(e => e.User)
                      .WithMany(u => u.ExternalLogins)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}