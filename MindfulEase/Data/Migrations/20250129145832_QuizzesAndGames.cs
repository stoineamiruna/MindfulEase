using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class QuizzesAndGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Quizzes",
                newName: "Result");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TherapeuticGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "GameUrl",
                table: "TherapeuticGames",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryMapping",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ApplicationUserQuizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "ApplicationUserQuizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionQuiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserQuestionQuizzes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    ResponseValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserQuestionQuizzes", x => new { x.UserId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserQuestionQuizzes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserQuestionQuizzes_QuestionQuiz_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionQuiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserQuestionQuizzes_QuestionId",
                table: "ApplicationUserQuestionQuizzes",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuiz_QuizId",
                table: "QuestionQuiz",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserQuestionQuizzes");

            migrationBuilder.DropTable(
                name: "QuestionQuiz");

            migrationBuilder.DropColumn(
                name: "GameUrl",
                table: "TherapeuticGames");

            migrationBuilder.DropColumn(
                name: "CategoryMapping",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ApplicationUserQuizzes");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "ApplicationUserQuizzes");

            migrationBuilder.RenameColumn(
                name: "Result",
                table: "Quizzes",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TherapeuticGames",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
