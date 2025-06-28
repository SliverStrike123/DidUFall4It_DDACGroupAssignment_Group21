using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class MinorCrashout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionIds",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "QuizModelId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizModelId",
                table: "Questions",
                column: "QuizModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_QuizModelId",
                table: "Questions",
                column: "QuizModelId",
                principalTable: "Quizzes",
                principalColumn: "QuizModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_QuizModelId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuizModelId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuizModelId",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "QuestionIds",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
