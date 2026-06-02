using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class ContentAnalyticsConfiguration : IEntityTypeConfiguration<ContentAnalytics>
    {
        public void Configure(EntityTypeBuilder<ContentAnalytics> entity)
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
        }
    }
}
