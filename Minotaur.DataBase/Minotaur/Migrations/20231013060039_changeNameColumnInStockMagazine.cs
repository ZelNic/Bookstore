using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class changeNameColumnInStockMagazine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockMmagazine",
                table: "StockMmagazine");

            migrationBuilder.RenameTable(
                name: "StockMmagazine",
                newName: "StockMagazine");

            migrationBuilder.RenameColumn(
                name: "IsOrder",
                table: "StockMagazine",
                newName: "IsNeed");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResponsiblePersonId",
                table: "StockMagazine",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockMagazine",
                table: "StockMagazine",
                column: "Id");
        }

        /// <inheritdoc />
    }
}
