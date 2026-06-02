using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class SeoMetadataConfiguration : IEntityTypeConfiguration<SeoMetadata>
    {
        public void Configure(EntityTypeBuilder<SeoMetadata> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            
            entity.Property(e => e.BlogPostId).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(60);
            entity.Property(e => e.Description).HasMaxLength(160);
            entity.Property(e => e.Keywords).HasMaxLength(255);
            entity.Property(e => e.CanonicalUrl).HasMaxLength(500);
            entity.Property(e => e.OgTitle).HasMaxLength(60);
            entity.Property(e => e.OgDescription).HasMaxLength(160);
            entity.Property(e => e.OgImage).HasMaxLength(500);
            entity.Property(e => e.TwitterCard).HasMaxLength(50);
            
            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            entity.HasIndex(e => e.BlogPostId).IsUnique();
            entity.HasIndex(e => e.IsDeleted);

            // Note: Relationship is configured in BlogPostConfiguration
            // to avoid duplicate/conflicting foreign key definitions

            // Configure StructuredData as JSON with simple value comparer
            entity.Property(e => e.StructuredData)
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                      v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, string>()
                  )
                  .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, string>>(
                      (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                      c => c.Aggregate(0, (a, kvp) => unchecked(a * 397 ^ kvp.Key.GetHashCode() ^ (kvp.Value != null ? kvp.Value.GetHashCode() : 0))),
                      c => new Dictionary<string, string>(c)
                  ));
        }
    }
}
