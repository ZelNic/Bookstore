using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class addNewColumnInOrder6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippedProducts",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }      
    }
}
