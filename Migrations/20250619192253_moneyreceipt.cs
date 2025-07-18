using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class moneyreceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoneyReceiptNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "NonCourseItems",
                columns: table => new
                {
                    NonCourseItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NonCourseItemNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NonCourseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonCourseItems", x => x.NonCourseItemId);
                });

            migrationBuilder.CreateTable(
                name: "MoneyReceipts",
                columns: table => new
                {
                    MoneyReceiptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoneyReceiptNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdmissionId = table.Column<int>(type: "int", nullable: true),
                    NonCourseItemId = table.Column<int>(type: "int", nullable: true),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    VisitorId = table.Column<int>(type: "int", nullable: true),
                    NonCourseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookQuantity = table.Column<int>(type: "int", nullable: true),
                    BookUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HardwareDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HardwareQuantity = table.Column<int>(type: "int", nullable: true),
                    HardwareUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConsultancyDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsultancyFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OthersDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OthersAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankOrMFSName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchOrAgentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardOrMFSNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFullPayment = table.Column<bool>(type: "bit", nullable: false),
                    IsInvoiceCreated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyReceipts", x => x.MoneyReceiptId);
                    table.ForeignKey(
                        name: "FK_MoneyReceipts_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "AdmissionId");
                    table.ForeignKey(
                        name: "FK_MoneyReceipts_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_MoneyReceipts_NonCourseItems_NonCourseItemId",
                        column: x => x.NonCourseItemId,
                        principalTable: "NonCourseItems",
                        principalColumn: "NonCourseItemId");
                    table.ForeignKey(
                        name: "FK_MoneyReceipts_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "VisitorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoneyReceipts_AdmissionId",
                table: "MoneyReceipts",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyReceipts_InvoiceId",
                table: "MoneyReceipts",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyReceipts_NonCourseItemId",
                table: "MoneyReceipts",
                column: "NonCourseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyReceipts_VisitorId",
                table: "MoneyReceipts",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoneyReceipts");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "NonCourseItems");
        }
    }
}
