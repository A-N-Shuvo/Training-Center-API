using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class cla1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_Employees_ExecutiveId",
                table: "Visitors");

            migrationBuilder.RenameColumn(
                name: "ExecutiveId",
                table: "Visitors",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Visitors_ExecutiveId",
                table: "Visitors",
                newName: "IX_Visitors_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_Employees_EmployeeId",
                table: "Visitors",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_Employees_EmployeeId",
                table: "Visitors");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Visitors",
                newName: "ExecutiveId");

            migrationBuilder.RenameIndex(
                name: "IX_Visitors_EmployeeId",
                table: "Visitors",
                newName: "IX_Visitors_ExecutiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_Employees_ExecutiveId",
                table: "Visitors",
                column: "ExecutiveId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
