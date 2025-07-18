using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class empchan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<bool>(
            //    name: "IsAvailable",
            //    table: "Employees",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);
            migrationBuilder.AddColumn<bool>(
        name: "TempIsAvailable",
        table: "Employees",
        type: "bit",
        nullable: false,
        defaultValue: false);

            // Convert existing data
            migrationBuilder.Sql(@"
        UPDATE Employees 
        SET TempIsAvailable = CASE 
            WHEN IsAvailable = 'yes' THEN 1
            WHEN IsAvailable = 'no' THEN 0
            ELSE 0
        END
    ");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Employees");

            // Rename the new column
            migrationBuilder.RenameColumn(
                name: "TempIsAvailable",
                table: "Employees",
                newName: "IsAvailable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IsAvailable",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
