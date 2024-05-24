using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Multiplify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Certifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessLogo",
                table: "BusinessInformations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "BusinessInformations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessLogo",
                table: "BusinessInformations");

            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "BusinessInformations");
        }
    }
}
