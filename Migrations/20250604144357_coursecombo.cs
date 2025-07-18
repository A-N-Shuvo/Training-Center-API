using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class coursecombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseCombos",
                columns: table => new
                {
                    CourseComboId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedCourse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCombos", x => x.CourseComboId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCombos");
        }
    }
}
