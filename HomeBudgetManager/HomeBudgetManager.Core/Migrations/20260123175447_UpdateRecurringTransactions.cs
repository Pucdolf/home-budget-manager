using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecurringTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "repetable_transaction_renew_interval",
                table: "repetable_transactions",
                newName: "next_run_date");

            migrationBuilder.AddColumn<int>(
                name: "interval_type",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "interval_value",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "repetable_transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "interval_type",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "interval_value",
                table: "repetable_transactions");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "repetable_transactions");

            migrationBuilder.RenameColumn(
                name: "next_run_date",
                table: "repetable_transactions",
                newName: "repetable_transaction_renew_interval");
        }
    }
}
