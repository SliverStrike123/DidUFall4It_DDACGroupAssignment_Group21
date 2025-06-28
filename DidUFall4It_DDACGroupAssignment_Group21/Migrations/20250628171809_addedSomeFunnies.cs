using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class addedSomeFunnies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepeatAttempts",
                table: "QuizReviews");

            migrationBuilder.DropColumn(
                name: "TotalAttempts",
                table: "QuizReviews");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "QuizReviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "QuizReviews");

            migrationBuilder.AddColumn<int>(
                name: "RepeatAttempts",
                table: "QuizReviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalAttempts",
                table: "QuizReviews",
                type: "int",
                nullable: true);
        }
    }
}
