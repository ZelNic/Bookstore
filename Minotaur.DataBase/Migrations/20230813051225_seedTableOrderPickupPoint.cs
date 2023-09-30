using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class seedTableOrderPickupPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "OrderPickupPoint");

            migrationBuilder.AddColumn<int>(
                name: "CountOfOrders",
                table: "OrderPickupPoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "OrderPickupPoint",
                columns: new[] { "PointId", "BuildingNumber", "City", "CountOfOrders", "Street", "WorkingHours" },
                values: new object[] { 1, "1", "Москва", 0, "Советская", "08:00 - 20:00" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderPickupPoint",
                keyColumn: "PointId",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "CountOfOrders",
                table: "OrderPickupPoint");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "OrderPickupPoint",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
