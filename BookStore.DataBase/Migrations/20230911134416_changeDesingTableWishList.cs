using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class changeDesingTableWishList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountProduct",
                table: "WishLists");

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 11, 16, 44, 16, 184, DateTimeKind.Unspecified).AddTicks(9233));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 7, 8, 59, 15, 506, DateTimeKind.Unspecified).AddTicks(7946));

            migrationBuilder.UpdateData(
                table: "WishLists",
                keyColumn: "WishListId",
                keyValue: 1,
                column: "CountProduct",
                value: 1);
        }
    }
}
