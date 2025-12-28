using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixedFoKeyForUsersandHouses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_houses_house_admin_id",
                table: "houses");

            migrationBuilder.CreateIndex(
                name: "IX_users_user_house_id",
                table: "users",
                column: "user_house_id");

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_admin_id",
                table: "houses",
                column: "house_admin_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users",
                column: "user_house_id",
                principalTable: "houses",
                principalColumn: "house_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_user_house_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_houses_house_admin_id",
                table: "houses");

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_admin_id",
                table: "houses",
                column: "house_admin_id",
                unique: true);
        }
    }
}
