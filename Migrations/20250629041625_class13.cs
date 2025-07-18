using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class class13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedOn",
                table: "VisitorEmployees",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VisitorEmployees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferDate",
                table: "VisitorEmployees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "VisitorEmployees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VisitorEmployees");

            migrationBuilder.DropColumn(
                name: "TransferDate",
                table: "VisitorEmployees");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "VisitorEmployees");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "VisitorEmployees",
                newName: "AssignedOn");
        }
    }
}
