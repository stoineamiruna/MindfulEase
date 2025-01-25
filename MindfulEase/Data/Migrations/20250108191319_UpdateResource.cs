using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class UpdateResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routines_AspNetUsers_UserId",
                table: "Routines");

            migrationBuilder.DropIndex(
                name: "IX_Routines_UserId",
                table: "Routines");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Routines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Routines");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Routines",
                newName: "RoutineDescription");

            migrationBuilder.RenameColumn(
                name: "DifficultyLevel",
                table: "Routines",
                newName: "ClusterId");

            migrationBuilder.AddColumn<int>(
                name: "RoutineId",
                table: "Resources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserResources",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    IsLiked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserResources", x => new { x.UserId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserResources_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserResources_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_RoutineId",
                table: "Resources",
                column: "RoutineId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserResources_ResourceId",
                table: "ApplicationUserResources",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_Routines_RoutineId",
                table: "Resources",
                column: "RoutineId",
                principalTable: "Routines",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Routines_RoutineId",
                table: "Resources");

            migrationBuilder.DropTable(
                name: "ApplicationUserResources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_RoutineId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "RoutineId",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "RoutineDescription",
                table: "Routines",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ClusterId",
                table: "Routines",
                newName: "DifficultyLevel");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Routines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Routines",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Routines_UserId",
                table: "Routines",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routines_AspNetUsers_UserId",
                table: "Routines",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
