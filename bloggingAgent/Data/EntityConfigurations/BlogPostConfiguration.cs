using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> entity)
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

            // Configure Tags as JSON with simple value comparer
            entity.Property(e => e.Tags)
                  .HasConversion(
                      v => string.Join(',', v),
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                  )
                  .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                      (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                      c => c.Aggregate(0, (a, v) => unchecked(a * 397 ^ v.GetHashCode())),
                      c => new List<string>(c)
                  ));

            // Relationships
            entity.HasOne(e => e.SeoMetadata)
                  .WithOne()
                  .HasForeignKey<SeoMetadata>(e => e.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Analytics)
                  .WithOne()
                  .HasForeignKey<ContentAnalytics>(e => e.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                  .WithMany(u => u.BlogPosts)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Comments)
                  .WithOne(c => c.BlogPost)
                  .HasForeignKey(c => c.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
