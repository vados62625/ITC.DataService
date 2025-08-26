using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.ReportService.Migrations
{
    /// <inheritdoc />
    public partial class EngineEngineType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EngineType",
                table: "Engines",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineType",
                table: "Engines");
        }
    }
}
