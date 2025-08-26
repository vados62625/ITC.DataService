using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.ReportService.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDefects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Engines",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<double>(
                name: "CageDefect",
                table: "Analyses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "InnerRingDefect",
                table: "Analyses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Misalignment",
                table: "Analyses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OuterRingDefect",
                table: "Analyses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RollingElementsDefect",
                table: "Analyses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Unbalance",
                table: "Analyses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CageDefect",
                table: "Analyses");

            migrationBuilder.DropColumn(
                name: "InnerRingDefect",
                table: "Analyses");

            migrationBuilder.DropColumn(
                name: "Misalignment",
                table: "Analyses");

            migrationBuilder.DropColumn(
                name: "OuterRingDefect",
                table: "Analyses");

            migrationBuilder.DropColumn(
                name: "RollingElementsDefect",
                table: "Analyses");

            migrationBuilder.DropColumn(
                name: "Unbalance",
                table: "Analyses");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Engines",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
