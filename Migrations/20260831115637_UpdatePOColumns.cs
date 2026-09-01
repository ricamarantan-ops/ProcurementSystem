using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcurementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePOColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PRNumber",
                table: "PurchaseOrders",
                newName: "DeliveryDate");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "PurchaseOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "PurchaseOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PRReference",
                table: "PurchaseOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "PurchaseOrders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "PurchaseOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PRReference",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "PurchaseOrders");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "PurchaseOrders",
                newName: "PRNumber");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "PurchaseOrders",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
