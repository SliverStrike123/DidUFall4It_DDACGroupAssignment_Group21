using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class updatedInfographicTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InfographicTitle",
                table: "InfographicFeedback",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InfographicTitle",
                table: "InfographicFeedback");
        }
    }
}
