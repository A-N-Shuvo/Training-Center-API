using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class class15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitorEmployees",
                table: "VisitorEmployees");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "VisitorEmployees",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitorEmployees",
                table: "VisitorEmployees",
                columns: new[] { "VisitorId", "EmployeeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitorEmployees",
                table: "VisitorEmployees");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "VisitorEmployees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitorEmployees",
                table: "VisitorEmployees",
                columns: new[] { "VisitorId", "EmployeeId", "CreatedDate" });
        }
    }
}
