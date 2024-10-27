using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PgManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class new21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InitDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InitDate",
                table: "Transactions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InitDate",
                table: "Transactions");
        }
    }
}
