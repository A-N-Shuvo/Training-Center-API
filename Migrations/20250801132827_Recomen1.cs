using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class Recomen1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Recommendations");

            migrationBuilder.AddColumn<string>(
                name: "RecommendationStatus",
                table: "Recommendations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendationStatus",
                table: "Recommendations");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Recommendations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
