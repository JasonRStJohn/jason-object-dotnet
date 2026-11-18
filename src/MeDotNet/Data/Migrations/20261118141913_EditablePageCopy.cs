using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeDotNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditablePageCopy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageTexts",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTexts", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCategories",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    WorkTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkProblem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkApproach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkLinkText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InquiryLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectsShortLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectsTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectsSubtitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCategories", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Badge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Meta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkHref = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEmployer = table.Column<bool>(type: "bit", nullable: false),
                    RecordProject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectCategories_CategoryKey",
                        column: x => x.CategoryKey,
                        principalTable: "ProjectCategories",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CategoryKey_SortOrder",
                table: "Projects",
                columns: new[] { "CategoryKey", "SortOrder" });

            // Today's /work and /projects copy, so the pages look identical on day one.
            Seed.PageCopySeed.Apply(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageTexts");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectCategories");
        }
    }
}
