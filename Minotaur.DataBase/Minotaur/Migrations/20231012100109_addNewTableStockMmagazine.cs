using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class addNewTableStockMmagazine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockMmagazine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponsiblePersonId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ShelfNumber = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOrder = table.Column<bool>(type: "bit", nullable: false),
                    ProductDataOnPurchase = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMmagazine", x => x.Id);
                });
        }

        /// <inheritdoc />
    }
}
