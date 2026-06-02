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
        }
    }
}
