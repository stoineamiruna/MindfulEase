using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class QuizzesAndGames2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserQuestionQuizzes_QuestionQuiz_QuestionId",
                table: "ApplicationUserQuestionQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionQuiz_Quizzes_QuizId",
                table: "QuestionQuiz");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionQuiz",
                table: "QuestionQuiz");

            migrationBuilder.RenameTable(
                name: "QuestionQuiz",
                newName: "QuestionQuizzes");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionQuiz_QuizId",
                table: "QuestionQuizzes",
                newName: "IX_QuestionQuizzes_QuizId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionQuizzes",
                table: "QuestionQuizzes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserQuestionQuizzes_QuestionQuizzes_QuestionId",
                table: "ApplicationUserQuestionQuizzes",
                column: "QuestionId",
                principalTable: "QuestionQuizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionQuizzes_Quizzes_QuizId",
                table: "QuestionQuizzes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserQuestionQuizzes_QuestionQuizzes_QuestionId",
                table: "ApplicationUserQuestionQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionQuizzes_Quizzes_QuizId",
                table: "QuestionQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionQuizzes",
                table: "QuestionQuizzes");

            migrationBuilder.RenameTable(
                name: "QuestionQuizzes",
                newName: "QuestionQuiz");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionQuizzes_QuizId",
                table: "QuestionQuiz",
                newName: "IX_QuestionQuiz_QuizId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionQuiz",
                table: "QuestionQuiz",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserQuestionQuizzes_QuestionQuiz_QuestionId",
                table: "ApplicationUserQuestionQuizzes",
                column: "QuestionId",
                principalTable: "QuestionQuiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionQuiz_Quizzes_QuizId",
                table: "QuestionQuiz",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id");
        }
    }
}
