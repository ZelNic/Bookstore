using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class newColumnInTableOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isCourierDelivery",
                table: "Order",
                newName: "IsCourierDelivery");

            migrationBuilder.AddColumn<int>(
                name: "OrderPickupPointId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderPickupPointId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 27, 7, 58, 53, 553, DateTimeKind.Unspecified).AddTicks(2696));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {           

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 26, 12, 27, 34, 95, DateTimeKind.Unspecified).AddTicks(4057));
        }
    }
}
