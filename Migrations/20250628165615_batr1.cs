using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class batr1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchTransfers_Visitors_VisitorId",
                table: "BatchTransfers");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "BatchTransfers",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_BatchTransfers_VisitorId",
                table: "BatchTransfers",
                newName: "IX_BatchTransfers_BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchTransfers_Batchs_BatchId",
                table: "BatchTransfers",
                column: "BatchId",
                principalTable: "Batchs",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchTransfers_Batchs_BatchId",
                table: "BatchTransfers");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "BatchTransfers",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_BatchTransfers_BatchId",
                table: "BatchTransfers",
                newName: "IX_BatchTransfers_VisitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchTransfers_Visitors_VisitorId",
                table: "BatchTransfers",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "VisitorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
