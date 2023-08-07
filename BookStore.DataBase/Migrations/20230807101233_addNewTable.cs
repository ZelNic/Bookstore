using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class addNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseHistory",
                table: "PurchaseHistory");

            migrationBuilder.DeleteData(
                table: "PurchaseHistory",
                keyColumn: "PurchaseId",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "PurchaseId",
                table: "PurchaseHistory",
                newName: "PurchaseAmount");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "PurchaseHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseAmount",
                table: "PurchaseHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "PurchaseHistory",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CurrentPosition",
                table: "PurchaseHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "PurchaseHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCount",
                table: "PurchaseHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductPrice",
                table: "PurchaseHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TravelHistory",
                table: "PurchaseHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "BooksTable",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseHistory",
                table: "PurchaseHistory",
                column: "OrderId");

            migrationBuilder.UpdateData(
                table: "BooksTable",
                keyColumn: "BookId",
                keyValue: 1,
                column: "Price",
                value: 417);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseHistory",
                table: "PurchaseHistory");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PurchaseHistory");

            migrationBuilder.DropColumn(
                name: "CurrentPosition",
                table: "PurchaseHistory");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "PurchaseHistory");

            migrationBuilder.DropColumn(
                name: "ProductCount",
                table: "PurchaseHistory");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "PurchaseHistory");

            migrationBuilder.DropColumn(
                name: "TravelHistory",
                table: "PurchaseHistory");

            migrationBuilder.RenameColumn(
                name: "PurchaseAmount",
                table: "PurchaseHistory",
                newName: "PurchaseId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "PurchaseHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseId",
                table: "PurchaseHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "BooksTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseHistory",
                table: "PurchaseHistory",
                column: "PurchaseId");

            migrationBuilder.UpdateData(
                table: "BooksTable",
                keyColumn: "BookId",
                keyValue: 1,
                column: "Price",
                value: "417");

            migrationBuilder.InsertData(
                table: "PurchaseHistory",
                columns: new[] { "PurchaseId", "ProductId", "PurchaseDate", "UserId" },
                values: new object[] { 1, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });
        }
    }
}
