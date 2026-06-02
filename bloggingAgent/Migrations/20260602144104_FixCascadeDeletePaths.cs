using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bloggingAgent.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeletePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentAnalytics_BlogPosts_BlogPostId",
                table: "ContentAnalytics");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentAnalytics_BlogPosts_BlogPostId",
                table: "ContentAnalytics",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentAnalytics_BlogPosts_BlogPostId",
                table: "ContentAnalytics");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentAnalytics_BlogPosts_BlogPostId",
                table: "ContentAnalytics",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
