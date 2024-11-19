using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class CommissionDatetimetoDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionDatetime",
                table: "ReportsEncoded");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CommissionDate",
                table: "ReportsEncoded",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionDate",
                table: "ReportsEncoded");

            migrationBuilder.AddColumn<DateTime>(
                name: "CommissionDatetime",
                table: "ReportsEncoded",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
