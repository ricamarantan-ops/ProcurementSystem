using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcurementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSupplierColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Suppliers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupplierID",
                table: "Suppliers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierID",
                table: "Suppliers");
        }
    }
}
