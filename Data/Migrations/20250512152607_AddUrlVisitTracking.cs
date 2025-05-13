using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlSamurai.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlVisitTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "url_visits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shortId = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: true),
                    visitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UrlId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_url_visits", x => x.id);
                    table.ForeignKey(
                        name: "FK_url_visits_urls_UrlId",
                        column: x => x.UrlId,
                        principalTable: "urls",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_url_visits_shortId",
                table: "url_visits",
                column: "shortId");

            migrationBuilder.CreateIndex(
                name: "IX_url_visits_UrlId",
                table: "url_visits",
                column: "UrlId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "url_visits");
        }
    }
}
