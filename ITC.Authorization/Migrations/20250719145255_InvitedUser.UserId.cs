using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FG.Authorization.Migrations
{
    /// <inheritdoc />
    public partial class InvitedUserUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "InvitedUser",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InvitedUser");
        }
    }
}
