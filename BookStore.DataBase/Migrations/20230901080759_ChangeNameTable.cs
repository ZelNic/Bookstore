using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseJournal",
                table: "WarehouseJournal");

            migrationBuilder.RenameTable(
                name: "WarehouseJournal",
                newName: "StockJournal");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "StockJournal",
                newName: "StockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockJournal",
                table: "StockJournal",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockJournal",
                table: "StockJournal");

            migrationBuilder.RenameTable(
                name: "StockJournal",
                newName: "WarehouseJournal");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "WarehouseJournal",
                newName: "StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseJournal",
                table: "WarehouseJournal",
                column: "Id");
        }
    }
}
