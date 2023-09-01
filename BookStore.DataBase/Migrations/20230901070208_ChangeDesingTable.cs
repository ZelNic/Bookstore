using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDesingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ResponsiblePerson",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "ShelfNumber",
                table: "Stocks",
                newName: "ResponsiblePersonId");

            migrationBuilder.CreateTable(
                name: "WarehouseJournal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ResponsiblePersonId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ShelfNumber = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseJournal", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1,
                column: "ResponsiblePersonId",
                value: 12);

            migrationBuilder.InsertData(
                table: "WarehouseJournal",
                columns: new[] { "Id", "Count", "Operation", "ProductId", "ResponsiblePersonId", "ShelfNumber", "StoreId" },
                values: new object[] { 1, 1, "Прием товара", 1, 12, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseJournal");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonId",
                table: "Stocks",
                newName: "ShelfNumber");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponsiblePerson",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Count", "ProductId", "ResponsiblePerson", "ShelfNumber" },
                values: new object[] { 1, 1, 12, 1 });
        }
    }
}
