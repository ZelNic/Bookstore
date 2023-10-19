using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class addNewColumnInOrder5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductData",
                table: "Orders",
                newName: "ProductSent");

            migrationBuilder.AddColumn<string>(
                name: "OrderedProducts",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

    }
}
