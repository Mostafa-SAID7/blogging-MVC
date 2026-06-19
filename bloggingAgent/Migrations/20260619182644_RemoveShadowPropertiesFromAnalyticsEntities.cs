using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bloggingAgent.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowPropertiesFromAnalyticsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration handles the removal of shadow properties BlogPostId1 that were
            // created by EF Core due to relationship configuration conflicts.
            // The shadow properties are not present in all deployment states, so we use
            // SQL to conditionally drop them only if they exist.

            // Drop foreign key constraints if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ContentAnalytics_BlogPosts_BlogPostId1')
                    ALTER TABLE [ContentAnalytics] DROP CONSTRAINT [FK_ContentAnalytics_BlogPosts_BlogPostId1];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SeoMetadata_BlogPosts_BlogPostId1')
                    ALTER TABLE [SeoMetadata] DROP CONSTRAINT [FK_SeoMetadata_BlogPosts_BlogPostId1];
            ");

            // Drop indexes if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ContentAnalytics_BlogPostId1' AND object_id = OBJECT_ID('ContentAnalytics'))
                    DROP INDEX [IX_ContentAnalytics_BlogPostId1] ON [ContentAnalytics];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SeoMetadata_BlogPostId1' AND object_id = OBJECT_ID('SeoMetadata'))
                    DROP INDEX [IX_SeoMetadata_BlogPostId1] ON [SeoMetadata];
            ");

            // Drop columns if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'BlogPostId1' AND object_id = OBJECT_ID('ContentAnalytics'))
                    ALTER TABLE [ContentAnalytics] DROP COLUMN [BlogPostId1];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'BlogPostId1' AND object_id = OBJECT_ID('SeoMetadata'))
                    ALTER TABLE [SeoMetadata] DROP COLUMN [BlogPostId1];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down migration not supported for this cleanup migration
        }
    }
}