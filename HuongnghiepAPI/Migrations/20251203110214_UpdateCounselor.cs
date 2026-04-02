using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HuongnghiepAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCounselor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Organization",
                table: "Counselors");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Counselors");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Counselors",
                newName: "Details");

            migrationBuilder.CreateTable(
                name: "CounselingSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CounselorId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    BookingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounselingSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_CounselingSchedules_Counselors_CounselorId",
                        column: x => x.CounselorId,
                        principalTable: "Counselors",
                        principalColumn: "CounselorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounselingSchedules_CounselorId",
                table: "CounselingSchedules",
                column: "CounselorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounselingSchedules");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Counselors",
                newName: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "Organization",
                table: "Counselors",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Counselors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
