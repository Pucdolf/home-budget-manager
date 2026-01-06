using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class fixedTransactionNamesDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "transactions",
                newName: "transaction_value");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "transactions",
                newName: "transaction_date");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transactions",
                newName: "transaction_id");

            migrationBuilder.RenameColumn(
                name: "categoryId",
                table: "categories",
                newName: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "transaction_value",
                table: "transactions",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "transaction_date",
                table: "transactions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "categories",
                newName: "categoryId");
        }
    }
}
