using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class addCategoriesDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    category_name = table.Column<string>(type: "TEXT", nullable: false),
                    category_description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.categoryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
