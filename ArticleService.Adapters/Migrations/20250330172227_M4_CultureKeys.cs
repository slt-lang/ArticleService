using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class M4_CultureKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CultureKey",
                schema: "ArticleService",
                table: "Articles",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CultureKey",
                schema: "ArticleService",
                table: "Articles");
        }
    }
}
