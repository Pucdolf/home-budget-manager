using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameDBFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "houses",
                newName: "house_name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "houses",
                newName: "house_description");

            migrationBuilder.RenameIndex(
                name: "IX_houses_name",
                table: "houses",
                newName: "IX_houses_house_name");

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses",
                column: "house_admin_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users",
                column: "user_house_id",
                principalTable: "houses",
                principalColumn: "house_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "house_name",
                table: "houses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "house_description",
                table: "houses",
                newName: "description");

            migrationBuilder.RenameIndex(
                name: "IX_houses_house_name",
                table: "houses",
                newName: "IX_houses_name");

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses",
                column: "house_admin_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users",
                column: "user_house_id",
                principalTable: "houses",
                principalColumn: "house_id");
        }
    }
}
