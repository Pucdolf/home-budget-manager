using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class refactorDBObjectNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_houses_house_admin_id",
                table: "houses");

            migrationBuilder.DropIndex(
                name: "IX_houses_house_name",
                table: "houses");

            migrationBuilder.RenameColumn(
                name: "transaction_value",
                table: "transactions",
                newName: "description_category");

            migrationBuilder.RenameColumn(
                name: "transaction_is_repetable",
                table: "transactions",
                newName: "transaction_is_repeatable");

            migrationBuilder.RenameColumn(
                name: "transaction_date",
                table: "transactions",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "transaction_category",
                table: "transactions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "transactions",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "user_house_id1",
                table: "users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "transaction_description",
                table: "transactions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "house_admin_id1",
                table: "houses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_admin_id1",
                table: "houses",
                column: "house_admin_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id1",
                table: "houses",
                column: "house_admin_id1",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id1",
                table: "houses");

            migrationBuilder.DropIndex(
                name: "IX_houses_house_admin_id1",
                table: "houses");

            migrationBuilder.DropColumn(
                name: "user_house_id1",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "house_admin_id1",
                table: "houses");

            migrationBuilder.RenameColumn(
                name: "transaction_is_repeatable",
                table: "transactions",
                newName: "transaction_is_repetable");

            migrationBuilder.RenameColumn(
                name: "description_category",
                table: "transactions",
                newName: "transaction_value");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "transactions",
                newName: "transaction_date");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "transactions",
                newName: "transaction_category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transactions",
                newName: "transaction_id");

            migrationBuilder.AlterColumn<string>(
                name: "transaction_description",
                table: "transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions",
                column: "transaction_from_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_admin_id",
                table: "houses",
                column: "house_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_name",
                table: "houses",
                column: "house_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses",
                column: "house_admin_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_transaction_from_user_id",
                table: "transactions",
                column: "transaction_from_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
