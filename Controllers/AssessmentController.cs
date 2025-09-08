using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;
using TrainingCenter_Api.Models.DTOs;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssessmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        [HttpGet("GetAssessments")]
        public async Task<ActionResult<IEnumerable<AssessmentDTO>>> GetAllAssessments()
        {
            var assessments = await _context.Assessments
                .Include(a => a.Batch)
                .Include(a => a.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(a => a.Trainee)
                    .ThenInclude(t => t.Registration)
                .Select(a => new AssessmentDTO
                {
                    AssessmentId = a.AssessmentId,
                    AssessmentDate = a.AssessmentDate,
                    BatchName = a.Batch != null ? a.Batch.BatchName : "N/A",
                    InstructorName = a.Instructor != null && a.Instructor.Employee != null ? a.Instructor.Employee.EmployeeName : "N/A",
                    TraineeName = a.Trainee != null && a.Trainee.Registration != null ? a.Trainee.Registration.TraineeName : "N/A",
                    AssessmentType = a.AssessmentType,
                    TheoreticalScore = a.TheoreticalScore,
                    PracticalScore = a.PracticalScore,
                    OverallScore = a.OverallScore
                })
                .ToListAsync();

            return Ok(assessments);
        }



        [HttpGet("GetAssessment/{id}")]
        public async Task<ActionResult<AssessmentDetailssDTO>> GetAssessment(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Trainee)
                    .ThenInclude(t => t.Registration)
                .Include(a => a.Batch)
                .Include(a => a.Instructor)
                    .ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            var dto = new AssessmentDetailssDTO
            {
                AssessmentId = assessment.AssessmentId,
                AssessmentDate = assessment.AssessmentDate,

                BatchId = assessment.BatchId,
                BatchName = assessment.Batch?.BatchName ?? "N/A",

                InstructorId = assessment.InstructorId,
                InstructorName = assessment.Instructor?.Employee?.EmployeeName ?? "N/A",

                TraineeId = assessment.TraineeId,
                traineeIDNo = assessment.Trainee?.TraineeIDNo,
                TraineeName = assessment.Trainee?.Registration?.TraineeName ?? "N/A",

                AssessmentType = assessment.AssessmentType,

                TheoreticalScore = assessment.TheoreticalScore,
                PracticalScore = assessment.PracticalScore,
                OverallScore = assessment.OverallScore,

                DaysPresent = assessment.DaysPresent,
                TotalDays = assessment.TotalDays,
                AttendancePercentage = assessment.AttendancePercentage,
                ParticipationLevel = assessment.ParticipationLevel,

                TechnicalSkillsRating = assessment.TechnicalSkillsRating,
                CommunicationSkillsRating = assessment.CommunicationSkillsRating,
                TeamworkRating = assessment.TeamworkRating,

                DisciplineRemarks = assessment.DisciplineRemarks,
                Punctuality = assessment.Punctuality,
                AttitudeRating = assessment.AttitudeRating,

                Strengths = assessment.Strengths,
                Weaknesses = assessment.Weaknesses,
                ImprovementAreas = assessment.ImprovementAreas,
                TrainerRemarks = assessment.TrainerRemarks,

                IsFinalized = assessment.IsFinalized
            };

            return Ok(dto);
        }




        [HttpPost("InsertAssessment")]
        public async Task<IActionResult> CreateAssessments([FromBody] AssessmentCreateDTO dto)
        {
            if (dto == null || dto.Assessments == null || !dto.Assessments.Any())
                return BadRequest("No assessment data provided.");

            var duplicateTraineeIds = new List<int>();

            foreach (var detail in dto.Assessments)
            {
                // Check if this assessment already exists
                bool exists = await _context.Assessments.AnyAsync(a =>
                    a.AssessmentDate == dto.AssessmentDate &&
                    a.BatchId == dto.BatchId &&
                    a.InstructorId == dto.InstructorId &&
                    a.TraineeId == detail.TraineeId);

                if (exists)
                {
                    duplicateTraineeIds.Add(detail.TraineeId);
                    continue; // Skip this one
                }

                var assessment = new Assessment
                {
                    AssessmentDate = dto.AssessmentDate,
                    BatchId = dto.BatchId,
                    InstructorId = dto.InstructorId,
                    TraineeId = detail.TraineeId,
                    AssessmentType = detail.AssessmentType,
                    TheoreticalScore = detail.TheoreticalScore,
                    PracticalScore = detail.PracticalScore,
                    OverallScore = (detail.TheoreticalScore + detail.PracticalScore) / 2m,
                    DaysPresent = detail.DaysPresent,
                    TotalDays = detail.TotalDays,
                    AttendancePercentage = detail.TotalDays > 0
                        ? Math.Round((detail.DaysPresent * 100m) / detail.TotalDays, 2)
                        : 0,
                    ParticipationLevel = detail.ParticipationLevel,
                    TechnicalSkillsRating = detail.TechnicalSkillsRating,
                    CommunicationSkillsRating = detail.CommunicationSkillsRating,
                    TeamworkRating = detail.TeamworkRating,
                    DisciplineRemarks = detail.DisciplineRemarks,
                    Punctuality = detail.Punctuality,
                    AttitudeRating = detail.AttitudeRating,
                    Strengths = detail.Strengths,
                    Weaknesses = detail.Weaknesses,
                    ImprovementAreas = detail.ImprovementAreas,
                    TrainerRemarks = detail.TrainerRemarks,
                    IsFinalized = detail.IsFinalized
                };

                _context.Assessments.Add(assessment);
            }

            await _context.SaveChangesAsync();

            if (duplicateTraineeIds.Any())
            {
                return Ok(new
                {
                    message = "Assessments created with some duplicates skipped.",
                    skippedTrainees = duplicateTraineeIds
                });
            }

            return Ok(new { message = "Assessments created successfully." });
        }

        [HttpPut("UpdateAssessment/{id}")]
        public async Task<IActionResult> UpdateAssessments([FromBody] AssessmentCreateDTO dto)
        {
            if (dto == null || dto.Assessments == null || !dto.Assessments.Any())
                return BadRequest("No assessment data provided.");

            foreach (var detail in dto.Assessments)
            {
                var existing = await _context.Assessments.FirstOrDefaultAsync(a =>
                    a.AssessmentDate == dto.AssessmentDate &&
                    a.BatchId == dto.BatchId &&
                    a.InstructorId == dto.InstructorId &&
                    a.TraineeId == detail.TraineeId);

                if (existing != null)
                {
                    existing.AssessmentType = detail.AssessmentType;
                    existing.TheoreticalScore = detail.TheoreticalScore;
                    existing.PracticalScore = detail.PracticalScore;
                    existing.OverallScore = (detail.TheoreticalScore + detail.PracticalScore) / 2m;

                    existing.DaysPresent = detail.DaysPresent;
                    existing.TotalDays = detail.TotalDays;
                    existing.AttendancePercentage = detail.TotalDays > 0
                        ? Math.Round((detail.DaysPresent * 100m) / detail.TotalDays, 2)
                        : 0;

                    existing.ParticipationLevel = detail.ParticipationLevel;
                    existing.TechnicalSkillsRating = detail.TechnicalSkillsRating;
                    existing.CommunicationSkillsRating = detail.CommunicationSkillsRating;
                    existing.TeamworkRating = detail.TeamworkRating;
                    existing.DisciplineRemarks = detail.DisciplineRemarks;
                    existing.Punctuality = detail.Punctuality;
                    existing.AttitudeRating = detail.AttitudeRating;
                    existing.Strengths = detail.Strengths;
                    existing.Weaknesses = detail.Weaknesses;
                    existing.ImprovementAreas = detail.ImprovementAreas;
                    existing.TrainerRemarks = detail.TrainerRemarks;
                    existing.IsFinalized = detail.IsFinalized;

                    _context.Assessments.Update(existing);
                }
                else
                {
                    // Optional: insert new if not found
                    // Skip for now as per your instruction
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Assessments updated successfully." });
        }



        // DELETE: api/Assessment/{id}
        [HttpDelete("DeleteAssessment/{id}")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var assessment = await _context.Assessments
                    .Include(a => a.Recommendations)
                    .FirstOrDefaultAsync(a => a.AssessmentId == id);

                if (assessment == null)
                    return NotFound();

                // Remove related recommendations first
                _context.Recommendations.RemoveRange(assessment.Recommendations);
                _context.Assessments.Remove(assessment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Assessment deleted successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Delete failed", error = ex.Message });
            }
        }         



        [HttpGet("GetInsTraiByBatch/{batchId}")]
        public async Task<ActionResult<object>> GetBatchWithDetails(int batchId)
        {
            // First get batch with instructor info
            var batch = await _context.Batches
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(b => b.BatchId == batchId);

            if (batch == null)
            {
                return NotFound();
            }

            // Then get all trainees for this batch
            var trainees = await _context.Trainees
                .Where(t => t.BatchId == batchId)
                .Include(t => t.Registration)
                .ToListAsync();

            var result = new
            {
                Instructor = batch.Instructor != null ? new
                {
                    InstructorId = batch.Instructor.InstructorId,
                    InstructorName = batch.Instructor.Employee?.EmployeeName
                } : null,

                Trainees = trainees.Select(t => new
                {
                    TraineeId = t.TraineeId,
                    TraineeName = t.Registration?.TraineeName
                }).ToList()
            };

            return Ok(result);
        }


        //[HttpGet("GetInsTraiByBatches/{batchId}")]
        //public async Task<ActionResult<object>> GetBatchWithDetailss(int batchId)
        //{
        //    var batch = await _context.Batches
        //        .Include(b => b.Instructor)
        //            .ThenInclude(i => i.Employee)
        //        .FirstOrDefaultAsync(b => b.BatchId == batchId);

        //    if (batch == null) return NotFound();

        //    var trainees = await _context.Trainees
        //        .Where(t => t.BatchId == batchId)
        //        .Include(t => t.Registration)
        //        .ToListAsync();

        //    var result = new
        //    {
        //        Instructor = batch.Instructor != null ? new
        //        {
        //            InstructorId = batch.Instructor.InstructorId,
        //            InstructorName = batch.Instructor.Employee?.EmployeeName
        //        } : null,

        //        Trainees = trainees.Select(t => new
        //        {
        //            TraineeId = t.TraineeId,
        //            TraineeName = t.Registration?.TraineeName,
        //            TraineeIDNo = t.TraineeIDNo// Add this line
        //        }).ToList()
        //    };

        //    return Ok(result);
        //}

        [HttpGet("GetInsTraiByBatches/{batchId}")]
        public async Task<ActionResult<object>> GetBatchWithDetailss(int batchId)
        {
            var batch = await _context.Batches
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(b => b.BatchId == batchId);

            if (batch == null) return NotFound();

            var trainees = await _context.Trainees
                .Where(t => t.BatchId == batchId)
                .Include(t => t.Registration) // Make sure this is included
                .ToListAsync();

            var result = new
            {
                Instructor = batch.Instructor != null ? new
                {
                    InstructorId = batch.Instructor.InstructorId,
                    InstructorName = batch.Instructor.Employee?.EmployeeName
                } : null,

                Trainees = trainees.Select(t => new
                {
                    TraineeId = t.TraineeId,
                    TraineeName = t.Registration?.TraineeName,
                    TraineeIDNo = t.TraineeIDNo,
                    ImagePath = t.Registration?.ImagePath // Add this line
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("AssessedTrainees/{batchId}")]
        public IActionResult GetAssessedTraineesByBatch(int batchId)
        {
            var traineeIds = _context.Assessments
                .Where(a => a.BatchId == batchId)
                .Select(a => a.TraineeId)
                .Distinct()
                .ToList();

            return Ok(traineeIds);
        }


        [HttpGet("GetTrainee/{traineeId}")]
        public async Task<IActionResult> GetTrainee(int traineeId)
        {
            var trainee = await _context.Trainees
                .Include(t => t.Registration)
                .FirstOrDefaultAsync(t => t.TraineeId == traineeId);

            if (trainee == null)
                return NotFound();

            return Ok(new
            {
                traineeId = trainee.TraineeId,
                traineeName = trainee.Registration?.TraineeName,
                traineeIDNo = trainee.TraineeIDNo,
                imagePath = trainee.Registration?.ImagePath
            });
        }

    }
}