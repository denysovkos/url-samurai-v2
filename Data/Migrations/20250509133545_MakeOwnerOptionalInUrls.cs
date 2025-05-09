using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlSamurai.Migrations
{
    /// <inheritdoc />
    public partial class MakeOwnerOptionalInUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ownerId",
                table: "urls",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_urls_ownerId",
                table: "urls",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_urls_shortId",
                table: "urls",
                column: "shortId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_urls_AspNetUsers_ownerId",
                table: "urls",
                column: "ownerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_urls_AspNetUsers_ownerId",
                table: "urls");

            migrationBuilder.DropIndex(
                name: "IX_urls_ownerId",
                table: "urls");

            migrationBuilder.DropIndex(
                name: "IX_urls_shortId",
                table: "urls");

            migrationBuilder.DropColumn(
                name: "ownerId",
                table: "urls");

            migrationBuilder.RenameColumn(
                name: "urlValue",
                table: "urls",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "numericId",
                table: "urls",
                newName: "numeric_id");
        }
    }
}
