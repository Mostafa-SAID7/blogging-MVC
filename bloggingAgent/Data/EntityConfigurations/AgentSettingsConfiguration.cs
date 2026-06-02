using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class AgentSettingsConfiguration : IEntityTypeConfiguration<AgentSettings>
    {
        public void Configure(EntityTypeBuilder<AgentSettings> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            
            entity.Property(e => e.DefaultAuthor).HasMaxLength(100);
            entity.Property(e => e.Theme).HasMaxLength(50);

            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.DefaultTags)
                  .HasConversion(
                      v => string.Join(',', v),
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                  )
                  .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                      (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                      c => c.Aggregate(0, (a, v) => unchecked(a * 397 ^ v.GetHashCode())),
                      c => new List<string>(c)
                  ));

            // Configure CustomSettings as JSON with simple value comparer
            entity.Property(e => e.CustomSettings)
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                      v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new Dictionary<string, object>()
                  )
                  .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, object>>(
                      (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                      c => c.Aggregate(0, (a, kvp) => unchecked(a * 397 ^ kvp.Key.GetHashCode() ^ (kvp.Value != null ? kvp.Value.GetHashCode() : 0))),
                      c => new Dictionary<string, object>(c)
                  ));

            entity.HasIndex(e => e.IsDeleted);
        }
    }
}
