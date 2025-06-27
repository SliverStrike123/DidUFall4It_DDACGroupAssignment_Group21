using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class QuizMakerPart2ElectricBoogaloo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answer = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionIds = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizModelId);
                });

            migrationBuilder.CreateTable(
                name: "QuizReviews",
                columns: table => new
                {
                    QuizReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: true),
                    AverageScore = table.Column<double>(type: "float", nullable: true),
                    HighestScore = table.Column<int>(type: "int", nullable: true),
                    LowestScore = table.Column<int>(type: "int", nullable: true),
                    TotalAttempts = table.Column<int>(type: "int", nullable: true),
                    RepeatAttempts = table.Column<int>(type: "int", nullable: true),
                    Reviews = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InformativeRatings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngagementRatings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuizModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizReviews", x => x.QuizReviewId);
                    table.ForeignKey(
                        name: "FK_QuizReviews_Quizzes_QuizModelId",
                        column: x => x.QuizModelId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizModelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_QuizModelId",
                table: "QuizReviews",
                column: "QuizModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QuizReviews");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
