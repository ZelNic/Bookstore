﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class newColumnInOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmationСode",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "ConfirmationСode",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationСode",
                table: "Order");
        }
    }
}
