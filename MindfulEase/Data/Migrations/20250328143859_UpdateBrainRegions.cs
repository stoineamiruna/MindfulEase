using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class UpdateBrainRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmotionRegions_BrainRegions_BrainRegionId",
                table: "EmotionRegions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmotionRegions_Emotions_EmotionId",
                table: "EmotionRegions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmotionRegions",
                table: "EmotionRegions");

            migrationBuilder.RenameTable(
                name: "EmotionRegions",
                newName: "EmotionBrainRegions");

            migrationBuilder.RenameIndex(
                name: "IX_EmotionRegions_BrainRegionId",
                table: "EmotionBrainRegions",
                newName: "IX_EmotionBrainRegions_BrainRegionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmotionBrainRegions",
                table: "EmotionBrainRegions",
                columns: new[] { "EmotionId", "BrainRegionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionBrainRegions_BrainRegions_BrainRegionId",
                table: "EmotionBrainRegions",
                column: "BrainRegionId",
                principalTable: "BrainRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionBrainRegions_Emotions_EmotionId",
                table: "EmotionBrainRegions",
                column: "EmotionId",
                principalTable: "Emotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmotionBrainRegions_BrainRegions_BrainRegionId",
                table: "EmotionBrainRegions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmotionBrainRegions_Emotions_EmotionId",
                table: "EmotionBrainRegions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmotionBrainRegions",
                table: "EmotionBrainRegions");

            migrationBuilder.RenameTable(
                name: "EmotionBrainRegions",
                newName: "EmotionRegions");

            migrationBuilder.RenameIndex(
                name: "IX_EmotionBrainRegions_BrainRegionId",
                table: "EmotionRegions",
                newName: "IX_EmotionRegions_BrainRegionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmotionRegions",
                table: "EmotionRegions",
                columns: new[] { "EmotionId", "BrainRegionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionRegions_BrainRegions_BrainRegionId",
                table: "EmotionRegions",
                column: "BrainRegionId",
                principalTable: "BrainRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionRegions_Emotions_EmotionId",
                table: "EmotionRegions",
                column: "EmotionId",
                principalTable: "Emotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
