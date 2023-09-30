using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class changeTypeInWishList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "WishLists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 12, 8, 9, 14, 746, DateTimeKind.Unspecified).AddTicks(9475));

            migrationBuilder.UpdateData(
                table: "WishLists",
                keyColumn: "WishListId",
                keyValue: 1,
                column: "ProductId",
                value: "1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {          

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 11, 16, 44, 16, 184, DateTimeKind.Unspecified).AddTicks(9233));

            migrationBuilder.UpdateData(
                table: "WishLists",
                keyColumn: "WishListId",
                keyValue: 1,
                column: "ProductId",
                value: 1);
        }
    }
}
