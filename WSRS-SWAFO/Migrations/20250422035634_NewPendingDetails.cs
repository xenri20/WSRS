using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class NewPendingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportsPending_AspNetUsers_FormatorId",
                table: "ReportsPending");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportsPending_Students_StudentNumber",
                table: "ReportsPending");

            migrationBuilder.DropIndex(
                name: "IX_ReportsPending_FormatorId",
                table: "ReportsPending");

            migrationBuilder.DropIndex(
                name: "IX_ReportsPending_StudentNumber",
                table: "ReportsPending");

            migrationBuilder.AlterColumn<int>(
                name: "FormatorId",
                table: "ReportsPending",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Formator",
                table: "ReportsPending",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ReportsPending",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formator",
                table: "ReportsPending");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ReportsPending");

            migrationBuilder.AlterColumn<string>(
                name: "FormatorId",
                table: "ReportsPending",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsPending_FormatorId",
                table: "ReportsPending",
                column: "FormatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsPending_StudentNumber",
                table: "ReportsPending",
                column: "StudentNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportsPending_AspNetUsers_FormatorId",
                table: "ReportsPending",
                column: "FormatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportsPending_Students_StudentNumber",
                table: "ReportsPending",
                column: "StudentNumber",
                principalTable: "Students",
                principalColumn: "StudentNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
