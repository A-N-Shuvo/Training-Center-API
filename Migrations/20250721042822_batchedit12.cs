using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class batchedit12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreviousInstructorIds",
                table: "Batches",
                newName: "PreviousInstructorIdsString");

            migrationBuilder.CreateTable(
                name: "BatchClassSchedule",
                columns: table => new
                {
                    BatchesBatchId = table.Column<int>(type: "int", nullable: false),
                    ClassSchedulesClassScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchClassSchedule", x => new { x.BatchesBatchId, x.ClassSchedulesClassScheduleId });
                    table.ForeignKey(
                        name: "FK_BatchClassSchedule_Batches_BatchesBatchId",
                        column: x => x.BatchesBatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchClassSchedule_ClassSchedules_ClassSchedulesClassScheduleId",
                        column: x => x.ClassSchedulesClassScheduleId,
                        principalTable: "ClassSchedules",
                        principalColumn: "ClassScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchClassSchedule_ClassSchedulesClassScheduleId",
                table: "BatchClassSchedule",
                column: "ClassSchedulesClassScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchClassSchedule");

            migrationBuilder.RenameColumn(
                name: "PreviousInstructorIdsString",
                table: "Batches",
                newName: "PreviousInstructorIds");
        }
    }
}
