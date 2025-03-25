using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class AddTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizTags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizTags", x => new { x.TagId, x.QuizId });
                    table.ForeignKey(
                        name: "FK_QuizTags_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceTags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTags", x => new { x.TagId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ResourceTags_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TherapeuticGameTags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false),
                    TherapeuticGameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapeuticGameTags", x => new { x.TagId, x.TherapeuticGameId });
                    table.ForeignKey(
                        name: "FK_TherapeuticGameTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TherapeuticGameTags_TherapeuticGames_TherapeuticGameId",
                        column: x => x.TherapeuticGameId,
                        principalTable: "TherapeuticGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizTags_QuizId",
                table: "QuizTags",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTags_ResourceId",
                table: "ResourceTags",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapeuticGameTags_TherapeuticGameId",
                table: "TherapeuticGameTags",
                column: "TherapeuticGameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizTags");

            migrationBuilder.DropTable(
                name: "ResourceTags");

            migrationBuilder.DropTable(
                name: "TherapeuticGameTags");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
