using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class UpdateUserObjectiveProgress2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserObjectiveProgresses_UserObjectives_UserObjectiveUserId_UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses");

            migrationBuilder.AlterColumn<string>(
                name: "UserObjectiveUserId",
                table: "UserObjectiveProgresses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserObjectiveId",
                table: "UserObjectiveProgresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserObjectiveProgresses_UserObjectives_UserObjectiveUserId_UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses",
                columns: new[] { "UserObjectiveUserId", "UserObjectiveObjectiveId" },
                principalTable: "UserObjectives",
                principalColumns: new[] { "UserId", "ObjectiveId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserObjectiveProgresses_UserObjectives_UserObjectiveUserId_UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses");

            migrationBuilder.AlterColumn<string>(
                name: "UserObjectiveUserId",
                table: "UserObjectiveProgresses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserObjectiveId",
                table: "UserObjectiveProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserObjectiveProgresses_UserObjectives_UserObjectiveUserId_UserObjectiveObjectiveId",
                table: "UserObjectiveProgresses",
                columns: new[] { "UserObjectiveUserId", "UserObjectiveObjectiveId" },
                principalTable: "UserObjectives",
                principalColumns: new[] { "UserId", "ObjectiveId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
