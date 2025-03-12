using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class AddApplicationUserEmotion2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserEmotions",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmotionId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoodValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserEmotions", x => new { x.UserId, x.EmotionId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserEmotions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserEmotions_Emotions_EmotionId",
                        column: x => x.EmotionId,
                        principalTable: "Emotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserEmotions_EmotionId",
                table: "ApplicationUserEmotions",
                column: "EmotionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserEmotions");
        }
    }
}
