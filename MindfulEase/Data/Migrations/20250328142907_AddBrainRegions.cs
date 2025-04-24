using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class AddBrainRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrainRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrainRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmotionRegions",
                columns: table => new
                {
                    EmotionId = table.Column<int>(type: "int", nullable: false),
                    BrainRegionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmotionRegions", x => new { x.EmotionId, x.BrainRegionId });
                    table.ForeignKey(
                        name: "FK_EmotionRegions_BrainRegions_BrainRegionId",
                        column: x => x.BrainRegionId,
                        principalTable: "BrainRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmotionRegions_Emotions_EmotionId",
                        column: x => x.EmotionId,
                        principalTable: "Emotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmotionRegions_BrainRegionId",
                table: "EmotionRegions",
                column: "BrainRegionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmotionRegions");

            migrationBuilder.DropTable(
                name: "BrainRegions");
        }
    }
}
