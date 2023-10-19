using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class changeNameColumnInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductSent",
                table: "Orders",
                newName: "MissingItems");
        }

        /// <inheritdoc />
     
    }
}
