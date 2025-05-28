using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlSamurai.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlValidityTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "validTill",
                table: "urls",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "validTill",
                table: "urls");
        }
    }
}
