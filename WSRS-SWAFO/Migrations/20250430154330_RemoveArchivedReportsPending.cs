using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class RemoveArchivedReportsPending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedReportsPending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedReportsPending",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportPendingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedReportsPending", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedReportsPending_ReportPendingId",
                table: "ArchivedReportsPending",
                column: "ReportPendingId",
                unique: true);
        }
    }
}
