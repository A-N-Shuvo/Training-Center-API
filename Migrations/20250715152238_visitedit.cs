using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class visitedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionDetails_Batchs_BatchId",
                table: "AdmissionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Batchs_BatchId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Batchs_ClassRooms_ClassRoomId",
                table: "Batchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Batchs_Courses_CourseId",
                table: "Batchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Batchs_Instructors_InstructorId",
                table: "Batchs");

            migrationBuilder.DropForeignKey(
                name: "FK_BatchTransfers_Batchs_BatchId",
                table: "BatchTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Batchs_BatchId",
                table: "ClassSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_LMSResourceAccesses_Batchs_BatchId",
                table: "LMSResourceAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Batchs_BatchId",
                table: "Recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Batchs_BatchId",
                table: "TraineeAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Batchs_BatchId",
                table: "Trainees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Batchs",
                table: "Batchs");

            migrationBuilder.RenameTable(
                name: "Batchs",
                newName: "Batches");

            migrationBuilder.RenameIndex(
                name: "IX_Batchs_InstructorId",
                table: "Batches",
                newName: "IX_Batches_InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_Batchs_CourseId",
                table: "Batches",
                newName: "IX_Batches_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Batchs_ClassRoomId",
                table: "Batches",
                newName: "IX_Batches_ClassRoomId");

            migrationBuilder.AddColumn<string>(
                name: "VisitorNo",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Batches",
                table: "Batches",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionDetails_Batches_BatchId",
                table: "AdmissionDetails",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Batches_BatchId",
                table: "Assessments",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_ClassRooms_ClassRoomId",
                table: "Batches",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "ClassRoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Courses_CourseId",
                table: "Batches",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Instructors_InstructorId",
                table: "Batches",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BatchTransfers_Batches_BatchId",
                table: "BatchTransfers",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Batches_BatchId",
                table: "ClassSchedules",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LMSResourceAccesses_Batches_BatchId",
                table: "LMSResourceAccesses",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Batches_BatchId",
                table: "Recommendations",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Batches_BatchId",
                table: "TraineeAttendances",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Batches_BatchId",
                table: "Trainees",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionDetails_Batches_BatchId",
                table: "AdmissionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Batches_BatchId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Batches_ClassRooms_ClassRoomId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Courses_CourseId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Instructors_InstructorId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_BatchTransfers_Batches_BatchId",
                table: "BatchTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Batches_BatchId",
                table: "ClassSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_LMSResourceAccesses_Batches_BatchId",
                table: "LMSResourceAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Batches_BatchId",
                table: "Recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Batches_BatchId",
                table: "TraineeAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Batches_BatchId",
                table: "Trainees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Batches",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "VisitorNo",
                table: "Visitors");

            migrationBuilder.RenameTable(
                name: "Batches",
                newName: "Batchs");

            migrationBuilder.RenameIndex(
                name: "IX_Batches_InstructorId",
                table: "Batchs",
                newName: "IX_Batchs_InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_Batches_CourseId",
                table: "Batchs",
                newName: "IX_Batchs_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Batches_ClassRoomId",
                table: "Batchs",
                newName: "IX_Batchs_ClassRoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Batchs",
                table: "Batchs",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionDetails_Batchs_BatchId",
                table: "AdmissionDetails",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Batchs_BatchId",
                table: "Assessments",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batchs_ClassRooms_ClassRoomId",
                table: "Batchs",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "ClassRoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batchs_Courses_CourseId",
                table: "Batchs",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batchs_Instructors_InstructorId",
                table: "Batchs",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BatchTransfers_Batchs_BatchId",
                table: "BatchTransfers",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Batchs_BatchId",
                table: "ClassSchedules",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LMSResourceAccesses_Batchs_BatchId",
                table: "LMSResourceAccesses",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Batchs_BatchId",
                table: "Recommendations",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Batchs_BatchId",
                table: "TraineeAttendances",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Batchs_BatchId",
                table: "Trainees",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
