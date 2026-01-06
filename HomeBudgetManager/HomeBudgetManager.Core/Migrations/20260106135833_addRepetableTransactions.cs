using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class addRepetableTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_house_id1",
                table: "users");

            migrationBuilder.DropColumn(
                name: "description_category",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "transactions",
                newName: "category_id");

            migrationBuilder.CreateTable(
                name: "repetable_transactions",
                columns: table => new
                {
                    repetable_transaction_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionId1 = table.Column<int>(type: "INTEGER", nullable: false),
                    repetable_transaction_renew_interval = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repetable_transactions", x => x.repetable_transaction_id);
                    table.ForeignKey(
                        name: "FK_repetable_transactions_transactions_TransactionId1",
                        column: x => x.TransactionId1,
                        principalTable: "transactions",
                        principalColumn: "transaction_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_category_id",
                table: "transactions",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions",
                column: "transaction_from_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_repetable_transactions_TransactionId1",
                table: "repetable_transactions",
                column: "TransactionId1");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_category_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "repetable_transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_category_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_transaction_from_user_id",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "transactions",
                newName: "CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "user_house_id1",
                table: "users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description_category",
                table: "transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
