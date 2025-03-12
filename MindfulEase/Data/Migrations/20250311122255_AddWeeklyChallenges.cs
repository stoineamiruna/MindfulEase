using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class AddWeeklyChallenges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardPoints",
                table: "WeeklyChallenges");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "WeeklyChallenges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RequiredJournalEntries",
                table: "WeeklyChallenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredPoints",
                table: "WeeklyChallenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredQuizzes",
                table: "WeeklyChallenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredResources",
                table: "WeeklyChallenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "WeeklyChallenges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ApplicationUserQuizzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserWeeklyChallenges",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WeeklyChallengeId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserWeeklyChallenges", x => new { x.UserId, x.WeeklyChallengeId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserWeeklyChallenges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserWeeklyChallenges_WeeklyChallenges_WeeklyChallengeId",
                        column: x => x.WeeklyChallengeId,
                        principalTable: "WeeklyChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserWeeklyChallenges_WeeklyChallengeId",
                table: "ApplicationUserWeeklyChallenges",
                column: "WeeklyChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserWeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "WeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "RequiredJournalEntries",
                table: "WeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "RequiredPoints",
                table: "WeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "RequiredQuizzes",
                table: "WeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "RequiredResources",
                table: "WeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "WeeklyChallenges");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ApplicationUserQuizzes");

            migrationBuilder.AddColumn<int>(
                name: "RewardPoints",
                table: "WeeklyChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
