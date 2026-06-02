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
            // Store doubles as floats in SQL Server (double precision)
            entity.Property(e => e.AverageReadTime).HasColumnType("float");
            entity.Property(e => e.BounceRate).HasColumnType("float");
            entity.HasIndex(e => e.BlogPostId).IsUnique();
            entity.HasIndex(e => e.LastUpdated);

            // Configure TrafficSources as JSON
            entity.Property(e => e.TrafficSources)
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                      v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, int>()
                  );

            // Add foreign key relationship to BlogPost
            entity.HasOne<BlogPost>()
                  .WithOne(b => b.Analytics)
                  .HasForeignKey<ContentAnalytics>(e => e.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
