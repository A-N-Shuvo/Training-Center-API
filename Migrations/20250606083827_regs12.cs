using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class regs12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistrationId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_RegistrationId",
                table: "Courses",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Registrations_RegistrationId",
                table: "Courses",
                column: "RegistrationId",
                principalTable: "Registrations",
                principalColumn: "RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Registrations_RegistrationId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_RegistrationId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "Courses");
        }
    }
}
