using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenter_Api.Migrations
{
    /// <inheritdoc />
    public partial class asses2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    AssessmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraineeId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssessmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TheoreticalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PracticalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DaysPresent = table.Column<int>(type: "int", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    ParticipationLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalSkillsRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunicationSkillsRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamworkRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisciplineRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Punctuality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttitudeRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weaknesses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImprovementAreas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainerRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_Assessments_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assessments_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assessments_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_BatchId",
                table: "Assessments",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_InstructorId",
                table: "Assessments",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_TraineeId",
                table: "Assessments",
                column: "TraineeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assessments");
        }
    }
}
