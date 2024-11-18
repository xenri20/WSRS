using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WSRS_SWAFO.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class PCCL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrafficReportsEncodeds_Offenses_OffenseId",
                table: "TrafficReportsEncodeds");

            migrationBuilder.DropForeignKey(
                name: "FK_TrafficReportsEncodeds_Students_StudentNumber",
                table: "TrafficReportsEncodeds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrafficReportsEncodeds",
                table: "TrafficReportsEncodeds");

            migrationBuilder.RenameTable(
                name: "TrafficReportsEncodeds",
                newName: "TrafficReportsEncoded");

            migrationBuilder.RenameIndex(
                name: "IX_TrafficReportsEncodeds_StudentNumber",
                table: "TrafficReportsEncoded",
                newName: "IX_TrafficReportsEncoded_StudentNumber");

            migrationBuilder.RenameIndex(
                name: "IX_TrafficReportsEncodeds_OffenseId",
                table: "TrafficReportsEncoded",
                newName: "IX_TrafficReportsEncoded_OffenseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrafficReportsEncoded",
                table: "TrafficReportsEncoded",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficReportsEncoded_Offenses_OffenseId",
                table: "TrafficReportsEncoded",
                column: "OffenseId",
                principalTable: "Offenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficReportsEncoded_Students_StudentNumber",
                table: "TrafficReportsEncoded",
                column: "StudentNumber",
                principalTable: "Students",
                principalColumn: "StudentNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrafficReportsEncoded_Offenses_OffenseId",
                table: "TrafficReportsEncoded");

            migrationBuilder.DropForeignKey(
                name: "FK_TrafficReportsEncoded_Students_StudentNumber",
                table: "TrafficReportsEncoded");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrafficReportsEncoded",
                table: "TrafficReportsEncoded");

            migrationBuilder.RenameTable(
                name: "TrafficReportsEncoded",
                newName: "TrafficReportsEncodeds");

            migrationBuilder.RenameIndex(
                name: "IX_TrafficReportsEncoded_StudentNumber",
                table: "TrafficReportsEncodeds",
                newName: "IX_TrafficReportsEncodeds_StudentNumber");

            migrationBuilder.RenameIndex(
                name: "IX_TrafficReportsEncoded_OffenseId",
                table: "TrafficReportsEncodeds",
                newName: "IX_TrafficReportsEncodeds_OffenseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrafficReportsEncodeds",
                table: "TrafficReportsEncodeds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficReportsEncodeds_Offenses_OffenseId",
                table: "TrafficReportsEncodeds",
                column: "OffenseId",
                principalTable: "Offenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficReportsEncodeds_Students_StudentNumber",
                table: "TrafficReportsEncodeds",
                column: "StudentNumber",
                principalTable: "Students",
                principalColumn: "StudentNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
