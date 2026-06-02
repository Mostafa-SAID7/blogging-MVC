using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(50).IsRequired(false); // Allow NULL for Icon
            entity.Property(e => e.Color).HasMaxLength(7).IsRequired(false); // Allow NULL for Color
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsActive);

            // Self-referencing relationship for hierarchical categories
            // Using NoAction to prevent circular cascade paths in SQL Server
            entity.HasOne(e => e.ParentCategory)
                  .WithMany(e => e.SubCategories)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
