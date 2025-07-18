using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class MR1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoneyReceipts_NonCourseItems_NonCourseItemId",
                table: "MoneyReceipts");

            migrationBuilder.DropTable(
                name: "NonCourseItems");

            migrationBuilder.DropIndex(
                name: "IX_MoneyReceipts_NonCourseItemId",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "BankOrMFSName",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "BookName",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "BookQuantity",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "BookUnitPrice",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "BranchOrAgentNo",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "CardOrMFSNumber",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "ConsultancyDetails",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "ConsultancyFee",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "HardwareQuantity",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "HardwareUnitPrice",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "NonCourseItemId",
                table: "MoneyReceipts");

            migrationBuilder.DropColumn(
                name: "OthersAmount",
                table: "MoneyReceipts");

            migrationBuilder.RenameColumn(
                name: "OthersDetails",
                table: "MoneyReceipts",
                newName: "MFSName");

            migrationBuilder.RenameColumn(
                name: "NonCourseType",
                table: "MoneyReceipts",
                newName: "DebitOrCreditCardNo");

            migrationBuilder.RenameColumn(
                name: "HardwareDetails",
                table: "MoneyReceipts",
                newName: "ChequeNo");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "MoneyReceipts",
                newName: "BankName");

            migrationBuilder.AlterColumn<string>(
                name: "MoneyReceiptNo",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MFSName",
                table: "MoneyReceipts",
                newName: "OthersDetails");

            migrationBuilder.RenameColumn(
                name: "DebitOrCreditCardNo",
                table: "MoneyReceipts",
                newName: "NonCourseType");

            migrationBuilder.RenameColumn(
                name: "ChequeNo",
                table: "MoneyReceipts",
                newName: "HardwareDetails");

            migrationBuilder.RenameColumn(
                name: "BankName",
                table: "MoneyReceipts",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "MoneyReceiptNo",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankOrMFSName",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookName",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookQuantity",
                table: "MoneyReceipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BookUnitPrice",
                table: "MoneyReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchOrAgentNo",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardOrMFSNumber",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsultancyDetails",
                table: "MoneyReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultancyFee",
                table: "MoneyReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HardwareQuantity",
                table: "MoneyReceipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HardwareUnitPrice",
                table: "MoneyReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NonCourseItemId",
                table: "MoneyReceipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OthersAmount",
                table: "MoneyReceipts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NonCourseItems",
                columns: table => new
                {
                    NonCourseItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonCourseItemNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NonCourseType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonCourseItems", x => x.NonCourseItemId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoneyReceipts_NonCourseItemId",
                table: "MoneyReceipts",
                column: "NonCourseItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyReceipts_NonCourseItems_NonCourseItemId",
                table: "MoneyReceipts",
                column: "NonCourseItemId",
                principalTable: "NonCourseItems",
                principalColumn: "NonCourseItemId");
        }
    }
}
