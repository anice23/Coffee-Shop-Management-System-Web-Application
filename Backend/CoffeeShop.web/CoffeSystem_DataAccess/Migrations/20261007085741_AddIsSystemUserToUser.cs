using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeSystem_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSystemUserToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemUser",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystemUser",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "isActive");
        }
    }
}
