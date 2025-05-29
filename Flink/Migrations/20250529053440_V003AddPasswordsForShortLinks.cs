using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flink.Migrations
{
    /// <inheritdoc />
    public partial class V003AddPasswordsForShortLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "ShortenedLinks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "ShortenedLinks");
        }
    }
}
