using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamecardsUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "cards",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_cards_UserId",
                table: "cards",
                newName: "IX_cards_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "cards",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_cards_user_id",
                table: "cards",
                newName: "IX_cards_UserId");
        }
    }
}
