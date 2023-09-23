using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class changeDesingBooksTableOnProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BooksTable");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: true),
                    ProductRating = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Author", "Category", "Description", "ISBN", "ImageURL", "Price", "ProductRating", "Title" },
                values: new object[] { 1, "Жауме Кабре", 1, "Человек просыпается неизвестно где - возможно, в больничной палате, но это неточно - и не помнит о себе вообще ничего. \"Зовите меня Измаил\", - предлагает он врачам, которых, за неимением других версий, нарекает Юрием Живаго и мадам Бовари.", "978-5-389-22890-0", null, 417, null, "И нас пожирает пламя" });

            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 23, 8, 4, 5, 859, DateTimeKind.Unspecified).AddTicks(6502));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "StockJournal",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2023, 9, 17, 14, 2, 56, 273, DateTimeKind.Unspecified).AddTicks(9102));
        }
    }
}
