using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HuongnghiepAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDifficultyAndCareerToMajor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Career",
                table: "Majors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Majors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Career",
                table: "Majors");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Majors");
        }
    }
}
