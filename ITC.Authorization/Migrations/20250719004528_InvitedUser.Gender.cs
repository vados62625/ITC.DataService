﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FG.Authorization.Migrations
{
    /// <inheritdoc />
    public partial class InvitedUserGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "InvitedUser",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "InvitedUser");
        }
    }
}
