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
                .HasColumnType("TEXT");
            entity.HasIndex(e => e.Key);
            entity.HasIndex(e => new { e.Key, e.Category });
            entity.HasIndex(e => e.ExpiresAt);
        }
    }
}
