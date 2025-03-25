using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class M2_Visits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleVisits",
                schema: "ArticleService",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Visits = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleVisits", x => new { x.ArticleId, x.Date });
                    table.ForeignKey(
                        name: "FK_ArticleVisits_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "ArticleService",
                        principalTable: "Articles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleVisits_ArticleId",
                schema: "ArticleService",
                table: "ArticleVisits",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleVisits_Date",
                schema: "ArticleService",
                table: "ArticleVisits",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleVisits",
                schema: "ArticleService");
        }
    }
}
