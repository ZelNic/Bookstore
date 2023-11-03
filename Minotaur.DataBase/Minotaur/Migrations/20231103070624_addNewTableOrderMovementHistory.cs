using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minotaur.Migrations
{
    /// <inheritdoc />
    public partial class addNewTableOrderMovementHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderMovementHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistoryOfСonversion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOfReceiving = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMovementHistory", x => x.Id);
                });
        }
    }
}
