using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class fixedDBHouseIdForTransactionCanBeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_houses_transaction_for_house_id",
                table: "transactions");

            migrationBuilder.AlterColumn<int>(
                name: "transaction_for_house_id",
                table: "transactions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_houses_transaction_for_house_id",
                table: "transactions",
                column: "transaction_for_house_id",
                principalTable: "houses",
                principalColumn: "house_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_houses_transaction_for_house_id",
                table: "transactions");

            migrationBuilder.AlterColumn<int>(
                name: "transaction_for_house_id",
                table: "transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_houses_transaction_for_house_id",
                table: "transactions",
                column: "transaction_for_house_id",
                principalTable: "houses",
                principalColumn: "house_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
