using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flink.Migrations
{
    /// <inheritdoc />
    public partial class V002ShortLinksWithExpirationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropColumn(
            name: "CreationDate",
            table: "ShortenedLinks");
          
          migrationBuilder.AddColumn<long>(
            name: "CreationDate",
            table: "ShortenedLinks",
            type: "BIGINT",
            nullable: false,
            defaultValue: DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            
            migrationBuilder.AddColumn<long>(
                name: "ActiveFrom",
                table: "ShortenedLinks",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ActiveUntil",
                table: "ShortenedLinks",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropColumn(
            name: "ActiveFrom",
            table: "ShortenedLinks");

          migrationBuilder.DropColumn(
            name: "ActiveUntil",
            table: "ShortenedLinks");
        }
    }
}
