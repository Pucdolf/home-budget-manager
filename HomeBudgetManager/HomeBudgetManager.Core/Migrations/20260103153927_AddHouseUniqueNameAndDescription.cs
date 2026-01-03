using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddHouseUniqueNameAndDescription : Migration
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

            migrationBuilder.AlterColumn<int>(
                name: "user_house_id",
                table: "users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "houses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "houses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_houses_name",
                table: "houses",
                column: "name",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_houses_user_house_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_houses_name",
                table: "houses");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "users");

            migrationBuilder.DropColumn(
                name: "description",
                table: "houses");

            migrationBuilder.DropColumn(
                name: "name",
                table: "houses");

            migrationBuilder.AlterColumn<int>(
                name: "user_house_id",
                table: "users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
