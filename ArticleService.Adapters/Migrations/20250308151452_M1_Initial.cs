using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArticleService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class M1_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ArticleService");

            migrationBuilder.CreateTable(
                name: "Articles",
                schema: "ArticleService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleHistory",
                schema: "ArticleService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleHistory_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "ArticleService",
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleHistory_ArticleId",
                schema: "ArticleService",
                table: "ArticleHistory",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleHistory_CreateDate",
                schema: "ArticleService",
                table: "ArticleHistory",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleHistory_Id",
                schema: "ArticleService",
                table: "ArticleHistory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreateDate",
                schema: "ArticleService",
                table: "Articles",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Id",
                schema: "ArticleService",
                table: "Articles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Name",
                schema: "ArticleService",
                table: "Articles",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleHistory",
                schema: "ArticleService");

            migrationBuilder.DropTable(
                name: "Articles",
                schema: "ArticleService");
        }
    }
}
