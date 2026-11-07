using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeSystem_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCafeSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CafeSettings",
                columns: table => new
                {
                    CafeSettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CafeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptFooter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CafeSettings", x => x.CafeSettingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CafeSettings");
        }
    }
}
