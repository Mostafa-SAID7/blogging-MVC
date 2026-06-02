using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class AgentMemoryConfiguration : IEntityTypeConfiguration<AgentMemory>
    {
        public void Configure(EntityTypeBuilder<AgentMemory> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Metadata)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new Dictionary<string, object>())
                .HasColumnType("TEXT")
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, object>>(
                    (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, kvp) => unchecked(a * 397 ^ kvp.Key.GetHashCode() ^ (kvp.Value != null ? kvp.Value.GetHashCode() : 0))),
                    c => new Dictionary<string, object>(c)
                ));
            entity.HasIndex(e => e.Key);
            entity.HasIndex(e => new { e.Key, e.Category });
            entity.HasIndex(e => e.ExpiresAt);
        }
    }
}
