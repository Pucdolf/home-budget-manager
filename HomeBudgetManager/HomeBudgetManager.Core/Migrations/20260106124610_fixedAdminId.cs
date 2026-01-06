using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class fixedAdminId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id1",
                table: "houses");

            migrationBuilder.RenameColumn(
                name: "house_admin_id1",
                table: "houses",
                newName: "house_admin");

            migrationBuilder.RenameIndex(
                name: "IX_houses_house_admin_id1",
                table: "houses",
                newName: "IX_houses_house_admin");

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin",
                table: "houses",
                column: "house_admin",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin",
                table: "houses");

            migrationBuilder.RenameColumn(
                name: "house_admin",
                table: "houses",
                newName: "house_admin_id1");

            migrationBuilder.RenameIndex(
                name: "IX_houses_house_admin",
                table: "houses",
                newName: "IX_houses_house_admin_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id1",
                table: "houses",
                column: "house_admin_id1",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
