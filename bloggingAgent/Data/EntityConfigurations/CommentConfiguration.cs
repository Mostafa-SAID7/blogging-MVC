using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggingAgent.Data.EntityConfigurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.BlogPostId, e.IsApproved, e.IsSpam });

            // Self-referencing relationship for nested comments
            entity.HasOne(e => e.ParentComment)
                  .WithMany()
                  .HasForeignKey(e => e.ParentCommentId)
                  .OnDelete(DeleteBehavior.NoAction);

            // Relationships
            entity.HasOne(e => e.BlogPost)
                  .WithMany(b => b.Comments)
                  .HasForeignKey(e => e.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                  .WithMany(u => u.Comments)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
