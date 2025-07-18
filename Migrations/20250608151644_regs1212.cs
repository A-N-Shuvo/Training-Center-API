using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class regs1212 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_CourseCombos_CourseComboId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "SelectedCourse",
                table: "Registrations");

            migrationBuilder.AlterColumn<int>(
                name: "CourseComboId",
                table: "Registrations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Registrations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_CourseId",
                table: "Registrations",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_CourseCombos_CourseComboId",
                table: "Registrations",
                column: "CourseComboId",
                principalTable: "CourseCombos",
                principalColumn: "CourseComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Courses_CourseId",
                table: "Registrations",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_CourseCombos_CourseComboId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Courses_CourseId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_CourseId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Registrations");

            migrationBuilder.AlterColumn<int>(
                name: "CourseComboId",
                table: "Registrations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCourse",
                table: "Registrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_CourseCombos_CourseComboId",
                table: "Registrations",
                column: "CourseComboId",
                principalTable: "CourseCombos",
                principalColumn: "CourseComboId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
