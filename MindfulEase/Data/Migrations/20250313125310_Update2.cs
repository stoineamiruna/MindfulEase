using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserEmotions",
                table: "ApplicationUserEmotions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserEmotions",
                table: "ApplicationUserEmotions",
                columns: new[] { "UserId", "EmotionId", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserEmotions",
                table: "ApplicationUserEmotions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserEmotions",
                table: "ApplicationUserEmotions",
                columns: new[] { "UserId", "EmotionId" });
        }
    }
}
