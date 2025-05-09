using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UrlSamurai.Migrations
{
    /// <inheritdoc />
    public partial class InitUrlsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "urls",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    urlValue = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    shortId = table.Column<string>(type: "text", nullable: true),
                    numericId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_urls", x => x.id);
                });
            
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION generate_short_id()
                RETURNS text AS $$
                DECLARE
                    characters text := 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
                    output text := '';
                    i integer := 0;
                    pos integer;
                BEGIN
                    FOR i IN 1..6 LOOP -- Adjust 6 for desired length
                        pos := floor(random() * length(characters) + 1);
                        output := output || substr(characters, pos, 1);
                    END LOOP;
                    RETURN output;
                END;
                $$ LANGUAGE plpgsql IMMUTABLE;  
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION set_short_id() RETURNS trigger AS $$
                BEGIN
                    IF NEW.""shortId"" IS NULL THEN
                        NEW.""shortId"" := generate_short_id();
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE TRIGGER urls_short_id
                BEFORE INSERT ON urls
                FOR EACH ROW
                EXECUTE FUNCTION set_short_id();
            ");
            
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX idx_urls_short_id ON urls(""shortId"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "urls");
        }
    }
}
