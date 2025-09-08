using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class trattend1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "TraineeAttendances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TraineeAttendances");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "MarkedTime",
                table: "TraineeAttendances",
                type: "time",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "AttendanceStatus",
                table: "TraineeAttendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "TraineeAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "TraineeAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendances_InstructorId",
                table: "TraineeAttendances",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAttendances_InvoiceId",
                table: "TraineeAttendances",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Instructors_InstructorId",
                table: "TraineeAttendances",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Invoices_InvoiceId",
                table: "TraineeAttendances",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Instructors_InstructorId",
                table: "TraineeAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Invoices_InvoiceId",
                table: "TraineeAttendances");

            migrationBuilder.DropIndex(
                name: "IX_TraineeAttendances_InstructorId",
                table: "TraineeAttendances");

            migrationBuilder.DropIndex(
                name: "IX_TraineeAttendances_InvoiceId",
                table: "TraineeAttendances");

            migrationBuilder.DropColumn(
                name: "AttendanceStatus",
                table: "TraineeAttendances");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "TraineeAttendances");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "TraineeAttendances");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MarkedTime",
                table: "TraineeAttendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "TraineeAttendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TraineeAttendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
