using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class ersure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfographicFeedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InfographicTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InformativeRating = table.Column<int>(type: "int", nullable: false),
                    EngagementRating = table.Column<int>(type: "int", nullable: false),
                    ClarityRating = table.Column<int>(type: "int", nullable: false),
                    RelevanceRating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InfographicModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfographicFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfographicFeedback_Infographics_InfographicModelId",
                        column: x => x.InfographicModelId,
                        principalTable: "Infographics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfographicFeedback_InfographicModelId",
                table: "InfographicFeedback",
                column: "InfographicModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfographicFeedback");
        }
    }
}
