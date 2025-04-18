using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class Article02_HistoryUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "ArticleService",
                table: "ArticleHistory",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "ArticleService",
                table: "ArticleHistory");
        }
    }
}
