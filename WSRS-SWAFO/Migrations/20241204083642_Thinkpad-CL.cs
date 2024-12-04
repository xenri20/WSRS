using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class ThinkpadCL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CommissionDatetime",
                table: "ReportsEncoded");

            migrationBuilder.AddColumn<int>(
                name: "CollegeId",
                table: "ReportsEncoded",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CommissionDate",
                table: "ReportsEncoded",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Course",
                table: "ReportsEncoded",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormatorId",
                table: "ReportsEncoded",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "HearingDate",
                table: "ReportsEncoded",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "StatusOfSanction",
                table: "ReportsEncoded",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Nature",
                table: "Offenses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Classification",
                table: "Offenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Colleges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colleges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_CollegeId",
                table: "ReportsEncoded",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_FormatorId",
                table: "ReportsEncoded",
                column: "FormatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportsEncoded_AspNetUsers_FormatorId",
                table: "ReportsEncoded",
                column: "FormatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportsEncoded_Colleges_CollegeId",
                table: "ReportsEncoded",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportsEncoded_AspNetUsers_FormatorId",
                table: "ReportsEncoded");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportsEncoded_Colleges_CollegeId",
                table: "ReportsEncoded");

            migrationBuilder.DropTable(
                name: "Colleges");

            migrationBuilder.DropIndex(
                name: "IX_ReportsEncoded_CollegeId",
                table: "ReportsEncoded");

            migrationBuilder.DropIndex(
                name: "IX_ReportsEncoded_FormatorId",
                table: "ReportsEncoded");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "ReportsEncoded");

            migrationBuilder.DropColumn(
                name: "CommissionDate",
                table: "ReportsEncoded");

            migrationBuilder.DropColumn(
                name: "Course",
                table: "ReportsEncoded");

            migrationBuilder.DropColumn(
                name: "FormatorId",
                table: "ReportsEncoded");

            migrationBuilder.DropColumn(
                name: "HearingDate",
                table: "ReportsEncoded");

            migrationBuilder.DropColumn(
                name: "StatusOfSanction",
                table: "ReportsEncoded");

            migrationBuilder.AddColumn<string>(
                name: "Course",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CommissionDatetime",
                table: "ReportsEncoded",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Nature",
                table: "Offenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Classification",
                table: "Offenses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
