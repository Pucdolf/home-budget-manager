using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class fixedSomeVariablesCannotBeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_category_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "transaction_from_user_id",
                table: "transactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "transactions",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions",
                newName: "IX_transactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_category_id",
                table: "transactions",
                newName: "IX_transactions_CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "categories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_CategoryId",
                table: "transactions",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_CategoryId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "transactions",
                newName: "transaction_from_user_id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "transactions",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_UserId",
                table: "transactions",
                newName: "IX_transactions_transaction_from_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_CategoryId",
                table: "transactions",
                newName: "IX_transactions_category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_category_id",
                table: "transactions",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "category_id",
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
