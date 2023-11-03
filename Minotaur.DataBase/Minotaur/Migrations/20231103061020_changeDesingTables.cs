using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class changeDesingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPosition",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TravelHistory",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "EndPosition",
                table: "WorkerReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartingPosition",
                table: "WorkerReviews",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
