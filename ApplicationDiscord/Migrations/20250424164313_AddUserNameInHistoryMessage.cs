using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDiscord.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameInHistoryMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "HistoryMessages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "HistoryMessages");
        }
    }
}
