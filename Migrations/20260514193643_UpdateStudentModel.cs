using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabYonetimSistemi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "StudentNumber",
                table: "Students",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentNumber",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
