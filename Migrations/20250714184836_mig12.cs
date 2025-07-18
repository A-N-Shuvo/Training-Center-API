using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdmissionId",
                table: "Trainees",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_AdmissionId",
                table: "Trainees",
                column: "AdmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Admissions_AdmissionId",
                table: "Trainees",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Admissions_AdmissionId",
                table: "Trainees");

            migrationBuilder.DropIndex(
                name: "IX_Trainees_AdmissionId",
                table: "Trainees");

            migrationBuilder.DropColumn(
                name: "AdmissionId",
                table: "Trainees");
        }
    }
}
