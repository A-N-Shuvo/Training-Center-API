using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class BatchVisitorTransfer1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchTransfers");

            migrationBuilder.DropTable(
                name: "VisitorEmployees");

            migrationBuilder.CreateTable(
                name: "batchTransfer_Junctions",
                columns: table => new
                {
                    BatchTransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraineeId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransferDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batchTransfer_Junctions", x => x.BatchTransferId);
                    table.ForeignKey(
                        name: "FK_batchTransfer_Junctions_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_batchTransfer_Junctions_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visitorTransfer_Junctions",
                columns: table => new
                {
                    VisitorTransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitorId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitorTransfer_Junctions", x => x.VisitorTransferId);
                    table.ForeignKey(
                        name: "FK_visitorTransfer_Junctions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_visitorTransfer_Junctions_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "VisitorId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_batchTransfer_Junctions_BatchId",
                table: "batchTransfer_Junctions",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_batchTransfer_Junctions_TraineeId",
                table: "batchTransfer_Junctions",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_visitorTransfer_Junctions_EmployeeId",
                table: "visitorTransfer_Junctions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_visitorTransfer_Junctions_VisitorId",
                table: "visitorTransfer_Junctions",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batchTransfer_Junctions");

            migrationBuilder.DropTable(
                name: "visitorTransfer_Junctions");

            migrationBuilder.CreateTable(
                name: "BatchTransfers",
                columns: table => new
                {
                    TraineeId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    BatchTransferId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransferDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchTransfers", x => new { x.TraineeId, x.BatchId });
                    table.ForeignKey(
                        name: "FK_BatchTransfers_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchTransfers_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitorEmployees",
                columns: table => new
                {
                    VisitorId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitorTransferId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorEmployees", x => new { x.VisitorId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_VisitorEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitorEmployees_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "VisitorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchTransfers_BatchId",
                table: "BatchTransfers",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorEmployees_EmployeeId",
                table: "VisitorEmployees",
                column: "EmployeeId");
        }
    }
}
