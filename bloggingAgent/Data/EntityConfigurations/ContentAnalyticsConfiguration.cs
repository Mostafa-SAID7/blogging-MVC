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

            // Configure TrafficSources as JSON with simple value comparer
            entity.Property(e => e.TrafficSources)
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                      v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, int>()
                  )
                  .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, int>>(
                      (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                      c => c.Aggregate(0, (a, kvp) => unchecked(a * 397 ^ kvp.Key.GetHashCode() ^ kvp.Value.GetHashCode())),
                      c => new Dictionary<string, int>(c)
                  ));
        }
    }
}
