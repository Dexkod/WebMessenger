using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDiscord.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipFromUserToAuthorizarionToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AuthorizationTokens_AuthorizationTokenId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AuthorizationTokenId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuthorizationTokenId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationTokens_UserId",
                table: "AuthorizationTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationTokens_Users_UserId",
                table: "AuthorizationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationTokens_Users_UserId",
                table: "AuthorizationTokens");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizationTokens_UserId",
                table: "AuthorizationTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorizationTokenId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthorizationTokenId",
                table: "Users",
                column: "AuthorizationTokenId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AuthorizationTokens_AuthorizationTokenId",
                table: "Users",
                column: "AuthorizationTokenId",
                principalTable: "AuthorizationTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
