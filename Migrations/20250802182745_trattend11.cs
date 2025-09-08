using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class trattend11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Invoices_InvoiceId",
                table: "TraineeAttendances");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "TraineeAttendances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Invoices_InvoiceId",
                table: "TraineeAttendances",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAttendances_Invoices_InvoiceId",
                table: "TraineeAttendances");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "TraineeAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAttendances_Invoices_InvoiceId",
                table: "TraineeAttendances",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
