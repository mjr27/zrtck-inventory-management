using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZRTCK.InventoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPositionToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_position",
                table: "people",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_position",
                table: "people");
        }
    }
}
