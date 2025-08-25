using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.ReportService.Migrations
{
    /// <inheritdoc />
    public partial class EngineStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EngineStatus",
                table: "Engines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Engines",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineStatus",
                table: "Engines");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Engines");
        }
    }
}
