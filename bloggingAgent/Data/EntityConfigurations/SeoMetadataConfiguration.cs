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
        }
    }
}
