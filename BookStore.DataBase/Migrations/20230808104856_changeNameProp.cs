using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class changeNameProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ctreet",
                table: "User",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "Ctreet",
                table: "Order",
                newName: "Street");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Street",
                table: "User",
                newName: "Ctreet");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Order",
                newName: "Ctreet");
        }
    }
}
