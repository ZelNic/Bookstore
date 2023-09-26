using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class fixColumnNameInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConfirmationСode",
                table: "Order",
                newName: "ConfirmationCode");

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 26, 12, 27, 34, 95, DateTimeKind.Unspecified).AddTicks(4057));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 26, 11, 29, 25, 667, DateTimeKind.Unspecified).AddTicks(1494));
        }
    }
}
