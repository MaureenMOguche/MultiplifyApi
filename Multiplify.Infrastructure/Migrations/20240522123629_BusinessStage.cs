using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Multiplify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BusinessStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Stage",
                table: "BusinessInformations",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Stage",
                table: "BusinessInformations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
