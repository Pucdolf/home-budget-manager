using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class FieldNameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "user_role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_role",
                table: "users",
                newName: "Role");
        }
    }
}
