using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_Formators.Migrations
{
    /// <inheritdoc />
    public partial class _915pm4112025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colleges",
                columns: table => new
                {
                    CollegeID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colleges", x => x.CollegeID);
                });

            migrationBuilder.CreateTable(
                name: "Offenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Classification = table.Column<int>(type: "int", nullable: false),
                    Nature = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentNumber = table.Column<int>(type: "int", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentNumber);
                });

            migrationBuilder.CreateTable(
                name: "ReportsEncoded",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OffenseId = table.Column<int>(type: "int", nullable: false),
                    CollegeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentNumber = table.Column<int>(type: "int", nullable: false),
                    CommissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HearingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Sanction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusOfSanction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsEncoded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportsEncoded_Colleges_CollegeID",
                        column: x => x.CollegeID,
                        principalTable: "Colleges",
                        principalColumn: "CollegeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportsEncoded_Offenses_OffenseId",
                        column: x => x.OffenseId,
                        principalTable: "Offenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportsEncoded_Students_StudentNumber",
                        column: x => x.StudentNumber,
                        principalTable: "Students",
                        principalColumn: "StudentNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportsPending",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsPending", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportsPending_Students_StudentNumber",
                        column: x => x.StudentNumber,
                        principalTable: "Students",
                        principalColumn: "StudentNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrafficReportsEncoded",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OffenseId = table.Column<int>(type: "int", nullable: false),
                    StudentNumber = table.Column<int>(type: "int", nullable: false),
                    CollegeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ORNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatePaid = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficReportsEncoded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafficReportsEncoded_Colleges_CollegeID",
                        column: x => x.CollegeID,
                        principalTable: "Colleges",
                        principalColumn: "CollegeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrafficReportsEncoded_Offenses_OffenseId",
                        column: x => x.OffenseId,
                        principalTable: "Offenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrafficReportsEncoded_Students_StudentNumber",
                        column: x => x.StudentNumber,
                        principalTable: "Students",
                        principalColumn: "StudentNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_CollegeID",
                table: "ReportsEncoded",
                column: "CollegeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_OffenseId",
                table: "ReportsEncoded",
                column: "OffenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_StudentNumber",
                table: "ReportsEncoded",
                column: "StudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsPending_StudentNumber",
                table: "ReportsPending",
                column: "StudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNumber",
                table: "Students",
                column: "StudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficReportsEncoded_CollegeID",
                table: "TrafficReportsEncoded",
                column: "CollegeID");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficReportsEncoded_OffenseId",
                table: "TrafficReportsEncoded",
                column: "OffenseId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficReportsEncoded_StudentNumber",
                table: "TrafficReportsEncoded",
                column: "StudentNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportsEncoded");

            migrationBuilder.DropTable(
                name: "ReportsPending");

            migrationBuilder.DropTable(
                name: "TrafficReportsEncoded");

            migrationBuilder.DropTable(
                name: "Colleges");

            migrationBuilder.DropTable(
                name: "Offenses");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
