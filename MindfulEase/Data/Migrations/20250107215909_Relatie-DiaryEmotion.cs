using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class RelatieDiaryEmotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserEmotions");

            migrationBuilder.CreateTable(
                name: "DiaryEmotions",
                columns: table => new
                {
                    DiaryId = table.Column<int>(type: "int", nullable: false),
                    EmotionId = table.Column<int>(type: "int", nullable: false),
                    Procent = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryEmotions", x => new { x.DiaryId, x.EmotionId });
                    table.ForeignKey(
                        name: "FK_DiaryEmotions_Diaries_DiaryId",
                        column: x => x.DiaryId,
                        principalTable: "Diaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiaryEmotions_Emotions_EmotionId",
                        column: x => x.EmotionId,
                        principalTable: "Emotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEmotions_EmotionId",
                table: "DiaryEmotions",
                column: "EmotionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaryEmotions");

            migrationBuilder.CreateTable(
                name: "ApplicationUserEmotions",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmotionId = table.Column<int>(type: "int", nullable: false)
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
    }
}
