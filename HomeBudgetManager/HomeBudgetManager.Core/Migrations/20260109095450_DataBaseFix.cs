using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class DataBaseFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: true),
                    category_name = table.Column<string>(type: "TEXT", nullable: false),
                    category_description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "houses",
                columns: table => new
                {
                    house_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    house_admin_id = table.Column<int>(type: "INTEGER", nullable: false),
                    house_name = table.Column<string>(type: "TEXT", nullable: false),
                    house_description = table.Column<string>(type: "TEXT", nullable: true),
                    house_join_code = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_houses", x => x.house_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_email = table.Column<string>(type: "TEXT", nullable: false),
                    user_login = table.Column<string>(type: "TEXT", nullable: false),
                    user_password = table.Column<string>(type: "TEXT", nullable: false),
                    user_role = table.Column<int>(type: "INTEGER", nullable: false),
                    user_house_id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_houses_user_house_id",
                        column: x => x.user_house_id,
                        principalTable: "houses",
                        principalColumn: "house_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    category_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    transaction_value = table.Column<decimal>(type: "TEXT", nullable: false),
                    transaction_type = table.Column<int>(type: "INTEGER", nullable: false),
                    transaction_description = table.Column<string>(type: "TEXT", nullable: true),
                    transaction_for_house_id = table.Column<int>(type: "INTEGER", nullable: true),
                    transaction_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    transaction_is_repeatable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_transactions_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_houses_transaction_for_house_id",
                        column: x => x.transaction_for_house_id,
                        principalTable: "houses",
                        principalColumn: "house_id");
                    table.ForeignKey(
                        name: "FK_transactions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "repetable_transactions",
                columns: table => new
                {
                    repetable_transaction_id = table.Column<int>(type: "INTEGER", nullable: false),
                    repetable_transaction_renew_interval = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repetable_transactions", x => x.repetable_transaction_id);
                    table.ForeignKey(
                        name: "FK_repetable_transactions_transactions_repetable_transaction_id",
                        column: x => x.repetable_transaction_id,
                        principalTable: "transactions",
                        principalColumn: "transaction_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_user_id",
                table: "categories",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_houses_house_admin_id",
                table: "houses",
                column: "house_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_category_id",
                table: "transactions",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_for_house_id",
                table: "transactions",
                column: "transaction_for_house_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                table: "transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_user_email",
                table: "users",
                column: "user_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_user_house_id",
                table: "users",
                column: "user_house_id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_users_user_id",
                table: "categories",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_houses_users_house_admin_id",
                table: "houses",
                column: "house_admin_id",
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

            migrationBuilder.DropTable(
                name: "repetable_transactions");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "houses");
        }
    }
}
