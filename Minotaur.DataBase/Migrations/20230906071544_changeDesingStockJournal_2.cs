using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class changeDesingStockJournal_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDataOnPurchase",
                table: "StockJournal",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ProductDataOnPurchase", "Time" },
                values: new object[] { null, new DateTime(2023, 9, 6, 10, 15, 44, 386, DateTimeKind.Unspecified).AddTicks(4095) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {          
            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 6, 10, 12, 25, 572, DateTimeKind.Unspecified).AddTicks(4328));
        }
    }
}
