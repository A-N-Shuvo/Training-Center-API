using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class LastUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Admissions_AdmissionId",
                table: "Recommendations");

            migrationBuilder.RenameColumn(
                name: "AdmissionId",
                table: "Recommendations",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendations_AdmissionId",
                table: "Recommendations",
                newName: "IX_Recommendations_BatchId");

            migrationBuilder.CreateTable(
                name: "BatchPlannings",
                columns: table => new
                {
                    BatchPlanningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    CourseComboId = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: false),
                    PlannedBatchCount = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchPlannings", x => x.BatchPlanningId);
                    table.ForeignKey(
                        name: "FK_BatchPlannings_CourseCombos_CourseComboId",
                        column: x => x.CourseComboId,
                        principalTable: "CourseCombos",
                        principalColumn: "CourseComboId");
                    table.ForeignKey(
                        name: "FK_BatchPlannings_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    DayId = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsHoliday = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Batchs_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batchs",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "DayId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "SlotID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LMSResourceAccesses",
                columns: table => new
                {
                    ResourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedByEmployeeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LMSResourceAccesses", x => x.ResourceId);
                    table.ForeignKey(
                        name: "FK_LMSResourceAccesses_Batchs_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batchs",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LMSResourceAccesses_Employees_UploadedByEmployeeId",
                        column: x => x.UploadedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "BatchPlanningInstructors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchPlanningId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchPlanningInstructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchPlanningInstructors_BatchPlannings_BatchPlanningId",
                        column: x => x.BatchPlanningId,
                        principalTable: "BatchPlannings",
                        principalColumn: "BatchPlanningId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchPlanningInstructors_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchPlanningInstructors_BatchPlanningId",
                table: "BatchPlanningInstructors",
                column: "BatchPlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchPlanningInstructors_InstructorId",
                table: "BatchPlanningInstructors",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchPlannings_CourseComboId",
                table: "BatchPlannings",
                column: "CourseComboId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchPlannings_CourseId",
                table: "BatchPlannings",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_BatchId",
                table: "ClassSchedules",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_DayId",
                table: "ClassSchedules",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_SlotId",
                table: "ClassSchedules",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_LMSResourceAccesses_BatchId",
                table: "LMSResourceAccesses",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_LMSResourceAccesses_UploadedByEmployeeId",
                table: "LMSResourceAccesses",
                column: "UploadedByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Batchs_BatchId",
                table: "Recommendations",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Batchs_BatchId",
                table: "Recommendations");

            migrationBuilder.DropTable(
                name: "BatchPlanningInstructors");

            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "LMSResourceAccesses");

            migrationBuilder.DropTable(
                name: "BatchPlannings");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "Recommendations",
                newName: "AdmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendations_BatchId",
                table: "Recommendations",
                newName: "IX_Recommendations_AdmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Admissions_AdmissionId",
                table: "Recommendations",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
