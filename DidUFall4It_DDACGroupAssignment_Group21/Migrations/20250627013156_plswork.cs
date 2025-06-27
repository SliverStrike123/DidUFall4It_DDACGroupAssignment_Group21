using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DidUFall4It_DDACGroupAssignment_Group21.Migrations
{
    /// <inheritdoc />
    public partial class plswork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CustomerFullName",
                table: "AspNetUsers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CustomerDOB",
                table: "AspNetUsers",
                newName: "DOB");

            migrationBuilder.RenameColumn(
                name: "CustomerAge",
                table: "AspNetUsers",
                newName: "Age");

            migrationBuilder.AlterColumn<string>(
                name: "Tips",
                table: "Infographics",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Infographics",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "CustomerFullName");

            migrationBuilder.RenameColumn(
                name: "DOB",
                table: "AspNetUsers",
                newName: "CustomerDOB");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "AspNetUsers",
                newName: "CustomerAge");

            migrationBuilder.AlterColumn<string>(
                name: "Tips",
                table: "Infographics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Infographics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
