using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "College",
                columns: table => new
                {
                    CollegeID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_College", x => x.CollegeID);
                });

            migrationBuilder.CreateTable(
                name: "Offenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nature = table.Column<int>(type: "int", nullable: false),
                    Classification = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentNumber);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    FormatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CommissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HearingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Sanction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusOfSanction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsEncoded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportsEncoded_AspNetUsers_FormatorId",
                        column: x => x.FormatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportsEncoded_College_CollegeID",
                        column: x => x.CollegeID,
                        principalTable: "College",
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
                    FormatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsPending", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportsPending_AspNetUsers_FormatorId",
                        column: x => x.FormatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CommissionDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ORNumber = table.Column<int>(type: "int", nullable: false),
                    DatePaid = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficReportsEncoded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafficReportsEncoded_College_CollegeID",
                        column: x => x.CollegeID,
                        principalTable: "College",
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_CollegeID",
                table: "ReportsEncoded",
                column: "CollegeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_FormatorId",
                table: "ReportsEncoded",
                column: "FormatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_OffenseId",
                table: "ReportsEncoded",
                column: "OffenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsEncoded_StudentNumber",
                table: "ReportsEncoded",
                column: "StudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsPending_FormatorId",
                table: "ReportsPending",
                column: "FormatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsPending_StudentNumber",
                table: "ReportsPending",
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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ReportsEncoded");

            migrationBuilder.DropTable(
                name: "ReportsPending");

            migrationBuilder.DropTable(
                name: "TrafficReportsEncoded");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "College");

            migrationBuilder.DropTable(
                name: "Offenses");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
