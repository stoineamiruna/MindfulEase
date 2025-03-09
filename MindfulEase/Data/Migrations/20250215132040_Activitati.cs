using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class Activitati : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "TherapeuticGames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Background",
                table: "TherapeuticGames");

            migrationBuilder.DropColumn(
                name: "Background",
                table: "Quizzes");
        }
    }
}
