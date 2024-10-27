using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PgManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class init11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "TaskDetails");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "TaskDetails");

            migrationBuilder.RenameColumn(
                name: "VideoPath",
                table: "TaskDetails",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "TaskDetails",
                newName: "AssignedToId");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MediaDescription",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadImage",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadVideo",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaDescription",
                table: "TaskDetails");

            migrationBuilder.DropColumn(
                name: "UploadImage",
                table: "TaskDetails");

            migrationBuilder.DropColumn(
                name: "UploadVideo",
                table: "TaskDetails");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "TaskDetails",
                newName: "VideoPath");

            migrationBuilder.RenameColumn(
                name: "AssignedToId",
                table: "TaskDetails",
                newName: "ProjectName");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "TaskDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
