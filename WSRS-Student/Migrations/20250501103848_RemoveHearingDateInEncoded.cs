using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHearingDateInEncoded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HearingDate",
                table: "ReportsEncoded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "HearingDate",
                table: "ReportsEncoded",
                type: "date",
                nullable: true);
        }
    }
}
