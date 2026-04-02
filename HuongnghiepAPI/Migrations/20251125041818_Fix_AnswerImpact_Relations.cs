using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HuongnghiepAPI.Migrations
{
    /// <inheritdoc />
    public partial class Fix_AnswerImpact_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerImpacts_Majors_MajorId1",
                table: "AnswerImpacts");

            migrationBuilder.DropIndex(
                name: "IX_AnswerImpacts_MajorId1",
                table: "AnswerImpacts");

            migrationBuilder.DropColumn(
                name: "MajorId1",
                table: "AnswerImpacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MajorId1",
                table: "AnswerImpacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnswerImpacts_MajorId1",
                table: "AnswerImpacts",
                column: "MajorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerImpacts_Majors_MajorId1",
                table: "AnswerImpacts",
                column: "MajorId1",
                principalTable: "Majors",
                principalColumn: "MajorId");
        }
    }
}
