using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class RepetableTransactionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_transactions_repetable_transaction_id",
                table: "repetable_transactions");

            migrationBuilder.RenameColumn(
                name: "repetable_transaction_id",
                table: "repetable_transactions",
                newName: "transaction_id");

            migrationBuilder.AddColumn<int>(
                name: "TransactionId1",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_repetable_transactions_TransactionId1",
                table: "repetable_transactions",
                column: "TransactionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_repetable_transactions_transactions_TransactionId1",
                table: "repetable_transactions",
                column: "TransactionId1",
                principalTable: "transactions",
                principalColumn: "transaction_id");

            migrationBuilder.AddForeignKey(
                name: "FK_repetable_transactions_transactions_transaction_id",
                table: "repetable_transactions",
                column: "transaction_id",
                principalTable: "transactions",
                principalColumn: "transaction_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_repetable_transactions_users_user_id",
                table: "repetable_transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_categories_category_id",
                table: "repetable_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_transactions_TransactionId1",
                table: "repetable_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_transactions_transaction_id",
                table: "repetable_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_repetable_transactions_users_user_id",
                table: "repetable_transactions");

            migrationBuilder.DropIndex(
                name: "IX_repetable_transactions_category_id",
                table: "repetable_transactions");

            migrationBuilder.DropIndex(
                name: "IX_repetable_transactions_TransactionId1",
                table: "repetable_transactions");

            migrationBuilder.DropIndex(
                name: "IX_repetable_transactions_user_id",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "TransactionId1",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "description",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "frequency_unit",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "next_run_date",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "value",
                table: "repetable_transactions");

            migrationBuilder.RenameColumn(
                name: "transaction_interval",
                table: "repetable_transactions",
                newName: "repetable_transaction_renew_interval");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "repetable_transactions",
                newName: "repetable_transaction_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "repetable_transaction_renew_interval",
                table: "repetable_transactions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_repetable_transactions_transactions_repetable_transaction_id",
                table: "repetable_transactions",
                column: "repetable_transaction_id",
                principalTable: "transactions",
                principalColumn: "transaction_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
