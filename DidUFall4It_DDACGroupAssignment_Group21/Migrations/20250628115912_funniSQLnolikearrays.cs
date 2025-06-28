using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class funniSQLnolikearrays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Options",
                table: "Questions",
                newName: "OptionTwo");

            migrationBuilder.AddColumn<string>(
                name: "OptionFour",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionOne",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionThree",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelectedAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttemptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InformativeRating = table.Column<int>(type: "int", nullable: false),
                    EngagementRating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "OptionFour",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "OptionOne",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "OptionThree",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "OptionTwo",
                table: "Questions",
                newName: "Options");
        }
    }
}
