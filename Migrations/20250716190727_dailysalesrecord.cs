using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class dailysalesrecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySalesMonitorings");

            migrationBuilder.CreateTable(
                name: "DailySalesRecords",
                columns: table => new
                {
                    DailySalesRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ColdCallsMade = table.Column<int>(type: "int", nullable: false),
                    MeetingsScheduled = table.Column<int>(type: "int", nullable: false),
                    MeetingsConducted = table.Column<int>(type: "int", nullable: false),
                    VisitorNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalkInsAttended = table.Column<int>(type: "int", nullable: false),
                    WalkInVisitorNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluationsAttended = table.Column<int>(type: "int", nullable: false),
                    CorporateVisitsScheduled = table.Column<int>(type: "int", nullable: false),
                    CorporateVisitsConducted = table.Column<int>(type: "int", nullable: false),
                    NewRegistrations = table.Column<int>(type: "int", nullable: false),
                    Enrollments = table.Column<int>(type: "int", nullable: false),
                    NewCollections = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueCollections = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySalesRecords", x => x.DailySalesRecordId);
                    table.ForeignKey(
                        name: "FK_DailySalesRecords_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailySalesRecords_EmployeeId",
                table: "DailySalesRecords",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySalesRecords");

            migrationBuilder.CreateTable(
                name: "DailySalesMonitorings",
                columns: table => new
                {
                    MonitoringId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AchievedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activities = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SalesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySalesMonitorings", x => x.MonitoringId);
                    table.ForeignKey(
                        name: "FK_DailySalesMonitorings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailySalesMonitorings_EmployeeId",
                table: "DailySalesMonitorings",
                column: "EmployeeId");
        }
    }
}
