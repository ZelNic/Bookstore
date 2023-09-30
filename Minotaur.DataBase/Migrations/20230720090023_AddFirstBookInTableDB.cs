using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstBookInTableDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BooksTable",
                columns: new[] { "Id", "Author", "Category", "Description", "ISBN", "Price", "Tittle" },
                values: new object[] { 1, "Жауме Кабре", "Художественная литература", "Человек просыпается неизвестно где - возможно, в больничной палате, но это неточно - и не помнит о себе вообще ничего. \"Зовите меня Измаил\", - предлагает он врачам, которых, за неимением других версий, нарекает Юрием Живаго и мадам Бовари.", "978-5-389-22890-0", "417", "И нас пожирает пламя" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BooksTable",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
