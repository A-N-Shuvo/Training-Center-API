using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class outtraatten2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendance_Batches_BatchId",
                table: "TraineeAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendance_Instructors_InstructorId",
                table: "TraineeAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetail_Admissions_AdmissionId",
                table: "TraineeAttendanceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetail_Invoices_InvoiceId",
                table: "TraineeAttendanceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetail_TraineeAttendance_TraineeAttendanceId",
                table: "TraineeAttendanceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetail_Trainees_TraineeId",
                table: "TraineeAttendanceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TraineeAttendanceDetail",
                table: "TraineeAttendanceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TraineeAttendance",
                table: "TraineeAttendance");

            migrationBuilder.RenameTable(
                name: "TraineeAttendanceDetail",
                newName: "TraineeAttendanceDetails");

            migrationBuilder.RenameTable(
                name: "TraineeAttendance",
                newName: "TraineeAttendances");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetail_TraineeId",
                table: "TraineeAttendanceDetails",
                newName: "IX_TraineeAttendanceDetails_TraineeId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetail_TraineeAttendanceId",
                table: "TraineeAttendanceDetails",
                newName: "IX_TraineeAttendanceDetails_TraineeAttendanceId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetail_InvoiceId",
                table: "TraineeAttendanceDetails",
                newName: "IX_TraineeAttendanceDetails_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetail_AdmissionId",
                table: "TraineeAttendanceDetails",
                newName: "IX_TraineeAttendanceDetails_AdmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendance_InstructorId",
                table: "TraineeAttendances",
                newName: "IX_TraineeAttendances_InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendance_BatchId",
                table: "TraineeAttendances",
                newName: "IX_TraineeAttendances_BatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraineeAttendanceDetails",
                table: "TraineeAttendanceDetails",
                column: "TraineeAttendanceDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraineeAttendances",
                table: "TraineeAttendances",
                column: "TraineeAttendanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetails_Admissions_AdmissionId",
                table: "TraineeAttendanceDetails",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetails_Invoices_InvoiceId",
                table: "TraineeAttendanceDetails",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetails_TraineeAttendances_TraineeAttendanceId",
                table: "TraineeAttendanceDetails",
                column: "TraineeAttendanceId",
                principalTable: "TraineeAttendances",
                principalColumn: "TraineeAttendanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetails_Trainees_TraineeId",
                table: "TraineeAttendanceDetails",
                column: "TraineeId",
                principalTable: "Trainees",
                principalColumn: "TraineeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Batches_BatchId",
                table: "TraineeAttendances",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Instructors_InstructorId",
                table: "TraineeAttendances",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetails_Admissions_AdmissionId",
                table: "TraineeAttendanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetails_Invoices_InvoiceId",
                table: "TraineeAttendanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetails_TraineeAttendances_TraineeAttendanceId",
                table: "TraineeAttendanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendanceDetails_Trainees_TraineeId",
                table: "TraineeAttendanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Batches_BatchId",
                table: "TraineeAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Instructors_InstructorId",
                table: "TraineeAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TraineeAttendances",
                table: "TraineeAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TraineeAttendanceDetails",
                table: "TraineeAttendanceDetails");

            migrationBuilder.RenameTable(
                name: "TraineeAttendances",
                newName: "TraineeAttendance");

            migrationBuilder.RenameTable(
                name: "TraineeAttendanceDetails",
                newName: "TraineeAttendanceDetail");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendances_InstructorId",
                table: "TraineeAttendance",
                newName: "IX_TraineeAttendance_InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendances_BatchId",
                table: "TraineeAttendance",
                newName: "IX_TraineeAttendance_BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetails_TraineeId",
                table: "TraineeAttendanceDetail",
                newName: "IX_TraineeAttendanceDetail_TraineeId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetails_TraineeAttendanceId",
                table: "TraineeAttendanceDetail",
                newName: "IX_TraineeAttendanceDetail_TraineeAttendanceId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetails_InvoiceId",
                table: "TraineeAttendanceDetail",
                newName: "IX_TraineeAttendanceDetail_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TraineeAttendanceDetails_AdmissionId",
                table: "TraineeAttendanceDetail",
                newName: "IX_TraineeAttendanceDetail_AdmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraineeAttendance",
                table: "TraineeAttendance",
                column: "TraineeAttendanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraineeAttendanceDetail",
                table: "TraineeAttendanceDetail",
                column: "TraineeAttendanceDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendance_Batches_BatchId",
                table: "TraineeAttendance",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendance_Instructors_InstructorId",
                table: "TraineeAttendance",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetail_Admissions_AdmissionId",
                table: "TraineeAttendanceDetail",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetail_Invoices_InvoiceId",
                table: "TraineeAttendanceDetail",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetail_TraineeAttendance_TraineeAttendanceId",
                table: "TraineeAttendanceDetail",
                column: "TraineeAttendanceId",
                principalTable: "TraineeAttendance",
                principalColumn: "TraineeAttendanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendanceDetail_Trainees_TraineeId",
                table: "TraineeAttendanceDetail",
                column: "TraineeId",
                principalTable: "Trainees",
                principalColumn: "TraineeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
