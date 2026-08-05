using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeDotNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostKindProjectSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Project",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Project",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Posts");
        }
    }
}
