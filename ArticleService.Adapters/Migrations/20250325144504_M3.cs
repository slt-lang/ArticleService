using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class M3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleVisits_Articles_ArticleId",
                schema: "ArticleService",
                table: "ArticleVisits");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleVisits_Articles_ArticleId",
                schema: "ArticleService",
                table: "ArticleVisits",
                column: "ArticleId",
                principalSchema: "ArticleService",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleVisits_Articles_ArticleId",
                schema: "ArticleService",
                table: "ArticleVisits");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleVisits_Articles_ArticleId",
                schema: "ArticleService",
                table: "ArticleVisits",
                column: "ArticleId",
                principalSchema: "ArticleService",
                principalTable: "Articles",
                principalColumn: "Id");
        }
    }
}
