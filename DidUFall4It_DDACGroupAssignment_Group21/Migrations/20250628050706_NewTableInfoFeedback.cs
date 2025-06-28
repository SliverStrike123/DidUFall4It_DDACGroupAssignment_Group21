using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class NewTableInfoFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfographicFeedback_Infographics_InfographicModelId",
                table: "InfographicFeedback");

            migrationBuilder.DropIndex(
                name: "IX_InfographicFeedback_InfographicModelId",
                table: "InfographicFeedback");

            migrationBuilder.DropColumn(
                name: "InfographicModelId",
                table: "InfographicFeedback");

            migrationBuilder.DropColumn(
                name: "InfographicTitle",
                table: "InfographicFeedback");

            migrationBuilder.AddColumn<int>(
                name: "InfographicId",
                table: "InfographicFeedback",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InfographicId",
                table: "InfographicFeedback");

            migrationBuilder.AddColumn<int>(
                name: "InfographicModelId",
                table: "InfographicFeedback",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfographicTitle",
                table: "InfographicFeedback",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InfographicFeedback_InfographicModelId",
                table: "InfographicFeedback",
                column: "InfographicModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_InfographicFeedback_Infographics_InfographicModelId",
                table: "InfographicFeedback",
                column: "InfographicModelId",
                principalTable: "Infographics",
                principalColumn: "Id");
        }
    }
}
