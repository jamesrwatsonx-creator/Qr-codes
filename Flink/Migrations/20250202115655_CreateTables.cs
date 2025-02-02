using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flink.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortenedLinks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Creator = table.Column<string>(type: "text", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortenedLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortLinkMetaData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    HttpStatusCode = table.Column<int>(type: "integer", nullable: false),
                    DocumentSize = table.Column<int>(type: "integer", nullable: false),
                    LastUpdate = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortLinkMetaData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortenedLinks");

            migrationBuilder.DropTable(
                name: "ShortLinkMetaData");
        }
    }
}
