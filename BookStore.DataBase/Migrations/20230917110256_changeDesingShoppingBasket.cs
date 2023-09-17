using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class changeDesingShoppingBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountProduct",
                table: "ShoppingBasket");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ShoppingBasket");

            migrationBuilder.AddColumn<string>(
                name: "ProductIdAndCount",
                table: "ShoppingBasket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ShoppingBasket",
                keyColumn: "BasketId",
                keyValue: 1,
                column: "ProductIdAndCount",
                value: "1:1");

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 17, 14, 2, 56, 273, DateTimeKind.Unspecified).AddTicks(9102));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {   

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 12, 8, 9, 14, 746, DateTimeKind.Unspecified).AddTicks(9475));
        }
    }
}
