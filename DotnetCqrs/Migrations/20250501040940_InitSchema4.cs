using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetCqrs.Migrations
{
    /// <inheritdoc />
    public partial class InitSchema4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Users",
                type: "time without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Users");
        }
    }
}
