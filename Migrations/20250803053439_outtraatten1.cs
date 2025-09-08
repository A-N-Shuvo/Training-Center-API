using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class outtraatten1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraineeAttendance",
                columns: table => new
                {
                    TraineeAttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeAttendance", x => x.TraineeAttendanceId);
                    table.ForeignKey(
                        name: "FK_TraineeAttendance_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeAttendance_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeAttendanceDetail",
                columns: table => new
                {
                    TraineeAttendanceDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraineeAttendanceId = table.Column<int>(type: "int", nullable: false),
                    TraineeId = table.Column<int>(type: "int", nullable: false),
                    AdmissionId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    AttendanceStatus = table.Column<bool>(type: "bit", nullable: false),
                    MarkedTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeAttendanceDetail", x => x.TraineeAttendanceDetailId);
                    table.ForeignKey(
                        name: "FK_TraineeAttendanceDetail_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "AdmissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeAttendanceDetail_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_TraineeAttendanceDetail_TraineeAttendance_TraineeAttendanceId",
                        column: x => x.TraineeAttendanceId,
                        principalTable: "TraineeAttendance",
                        principalColumn: "TraineeAttendanceId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TraineeAttendanceDetail_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendance_BatchId",
                table: "TraineeAttendance",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendance_InstructorId",
                table: "TraineeAttendance",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendanceDetail_AdmissionId",
                table: "TraineeAttendanceDetail",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendanceDetail_InvoiceId",
                table: "TraineeAttendanceDetail",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendanceDetail_TraineeAttendanceId",
                table: "TraineeAttendanceDetail",
                column: "TraineeAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendanceDetail_TraineeId",
                table: "TraineeAttendanceDetail",
                column: "TraineeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraineeAttendanceDetail");

            migrationBuilder.DropTable(
                name: "TraineeAttendance");
        }
    }
}
