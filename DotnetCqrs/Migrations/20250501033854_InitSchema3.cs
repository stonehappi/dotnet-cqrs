using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetCqrs.Migrations
{
    /// <inheritdoc />
    public partial class InitSchema3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Users");
        }
    }
}
