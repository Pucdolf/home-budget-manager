using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBudgetManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddHouseJoinCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "house_join_code",
                table: "houses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "house_join_code",
                table: "houses");
        }
    }
}
