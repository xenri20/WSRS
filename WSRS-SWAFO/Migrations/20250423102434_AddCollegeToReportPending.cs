using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class AddCollegeToReportPending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "College",
                table: "ReportsPending",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CourseYearSection",
                table: "ReportsPending",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "College",
                table: "ReportsPending");

            migrationBuilder.DropColumn(
                name: "CourseYearSection",
                table: "ReportsPending");
        }
    }
}
