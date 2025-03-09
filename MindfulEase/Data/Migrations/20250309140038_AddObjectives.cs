using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class AddObjectives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Objectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserObjectives",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ObjectiveId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    TargetValue = table.Column<int>(type: "int", nullable: true),
                    TargetTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserObjectives", x => new { x.UserId, x.ObjectiveId });
                    table.ForeignKey(
                        name: "FK_UserObjectives_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserObjectives_Objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "Objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserObjectiveProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserObjectiveId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    UserObjectiveUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserObjectiveObjectiveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserObjectiveProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserObjectiveProgresses_UserObjectives_UserObjectiveUserId_UserObjectiveObjectiveId",
                        columns: x => new { x.UserObjectiveUserId, x.UserObjectiveObjectiveId },
                        principalTable: "UserObjectives",
                        principalColumns: new[] { "UserId", "ObjectiveId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserObjectiveProgresses_UserObjectiveUserId_UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses",
                columns: new[] { "UserObjectiveUserId", "UserObjectiveObjectiveId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserObjectives_ObjectiveId",
                table: "UserObjectives",
                column: "ObjectiveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserObjectiveProgresses");

            migrationBuilder.DropTable(
                name: "UserObjectives");

            migrationBuilder.DropTable(
                name: "Objectives");
        }
    }
}
