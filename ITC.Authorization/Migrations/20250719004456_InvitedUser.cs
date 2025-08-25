using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FG.Authorization.Migrations
{
    /// <inheritdoc />
    public partial class InvitedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentDate",
                table: "InvitedUser");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "InvitedUser");

            migrationBuilder.DropColumn(
                name: "NumberEmploymentOrder",
                table: "InvitedUser");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "InvitedUser",
                newName: "EmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "InvitedUser",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "InvitedUser",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "InvitedUser",
                newName: "PersonId");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "InvitedUser",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "InvitedUser",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmploymentDate",
                table: "InvitedUser",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "InvitedUser",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumberEmploymentOrder",
                table: "InvitedUser",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
