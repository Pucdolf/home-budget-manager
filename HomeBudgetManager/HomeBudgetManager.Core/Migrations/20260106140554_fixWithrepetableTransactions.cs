using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class fixWithrepetableTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_transactions_TransactionId1",
                table: "repetable_transactions");

            migrationBuilder.DropIndex(
                name: "IX_repetable_transactions_TransactionId1",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "TransactionId1",
                table: "repetable_transactions");

            migrationBuilder.AlterColumn<int>(
                name: "repetable_transaction_id",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_repetable_transactions_transactions_repetable_transaction_id",
                table: "repetable_transactions",
                column: "repetable_transaction_id",
                principalTable: "transactions",
                principalColumn: "transaction_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_transactions_repetable_transaction_id",
                table: "repetable_transactions");

            migrationBuilder.AlterColumn<int>(
                name: "repetable_transaction_id",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId1",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_repetable_transactions_TransactionId1",
                table: "repetable_transactions",
                column: "TransactionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_repetable_transactions_transactions_TransactionId1",
                table: "repetable_transactions",
                column: "TransactionId1",
                principalTable: "transactions",
                principalColumn: "transaction_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
