using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "transaction_value",
                table: "transactions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_for_house_id",
                table: "transactions",
                column: "transaction_for_house_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions",
                column: "transaction_from_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_admin_id",
                table: "houses",
                column: "house_admin_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses",
                column: "house_admin_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_houses_transaction_for_house_id",
                table: "transactions",
                column: "transaction_for_house_id",
                principalTable: "houses",
                principalColumn: "house_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_transaction_from_user_id",
                table: "transactions",
                column: "transaction_from_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_houses_transaction_for_house_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_transaction_for_house_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_houses_house_admin_id",
                table: "houses");

            migrationBuilder.AlterColumn<int>(
                name: "transaction_value",
                table: "transactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }
    }
}
