using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class batchControllerupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Batches_BatchId",
                table: "ClassSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ClassSchedules_BatchId",
                table: "ClassSchedules");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "ClassSchedules");

            migrationBuilder.AddColumn<string>(
                name: "SelectedClassSchedules",
                table: "Batches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedClassSchedules",
                table: "Batches");

            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "ClassSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_BatchId",
                table: "ClassSchedules",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Batches_BatchId",
                table: "ClassSchedules",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId");
        }
    }
}
