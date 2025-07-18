using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchPlanningController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BatchPlanningController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: All BatchPlannings with Instructor IDs
        [HttpGet("GetBatchPlannings")]
        public async Task<ActionResult<IEnumerable<object>>> GetBatchPlannings()
        {
            var batchPlannings = await _context.BatchPlannings
                .Include(bp => bp.BatchPlanningInstructors)
                    .ThenInclude(bpi => bpi.Instructor)
                        .ThenInclude(i => i.Employee)
                .ToListAsync();

            var result = batchPlannings.Select(bp => new
            {
                bp.BatchPlanningId,
                bp.CourseId,
                bp.CourseComboId,
                bp.Year,
                bp.StartMonth,
                bp.DurationMonths,
                bp.PlannedBatchCount,
                bp.Remarks,
                bp.CreatedDate,
                bp.LastModifiedDate,
                InstructorIds = bp.BatchPlanningInstructors.Select(bpi => bpi.InstructorId),
                Instructors = bp.BatchPlanningInstructors.Select(bpi => new
                {
                    bpi.Instructor.InstructorId,
                    bpi.Instructor.Employee?.EmployeeName
                })
            });

            return Ok(result);
        }

        // ✅ POST: Create new BatchPlanning with Instructor assignments
        [HttpPost("Create")]
        public async Task<ActionResult> CreateBatchPlanning(BatchPlanning batchPlanning, [FromQuery] List<int> instructorIds)
        {
            _context.BatchPlannings.Add(batchPlanning);
            await _context.SaveChangesAsync();

            foreach (var instructorId in instructorIds)
            {
                _context.BatchPlanningInstructors.Add(new BatchPlanningInstructor
                {
                    BatchPlanningId = batchPlanning.BatchPlanningId,
                    InstructorId = instructorId
                });
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBatchPlannings), new { id = batchPlanning.BatchPlanningId }, batchPlanning);
        }

        // ✅ PUT: Update BatchPlanning and Instructor assignments
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateBatchPlanning(int id, BatchPlanning updatedPlanning, [FromQuery] List<int> instructorIds)
        {
            if (id != updatedPlanning.BatchPlanningId)
                return BadRequest("ID mismatch");

            var existing = await _context.BatchPlannings
                .Include(bp => bp.BatchPlanningInstructors)
                .FirstOrDefaultAsync(bp => bp.BatchPlanningId == id);

            if (existing == null)
                return NotFound();

            // Update fields
            existing.CourseId = updatedPlanning.CourseId;
            existing.CourseComboId = updatedPlanning.CourseComboId;
            existing.Year = updatedPlanning.Year;
            existing.StartMonth = updatedPlanning.StartMonth;
            existing.DurationMonths = updatedPlanning.DurationMonths;
            existing.PlannedBatchCount = updatedPlanning.PlannedBatchCount;
            existing.Remarks = updatedPlanning.Remarks;
            existing.LastModifiedDate = DateTime.Now;

            // Update instructors
            _context.BatchPlanningInstructors.RemoveRange(existing.BatchPlanningInstructors);
            foreach (var instructorId in instructorIds)
            {
                _context.BatchPlanningInstructors.Add(new BatchPlanningInstructor
                {
                    BatchPlanningId = id,
                    InstructorId = instructorId
                });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: BatchPlanning and its Instructor assignments
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteBatchPlanning(int id)
        {
            var batchPlanning = await _context.BatchPlannings
                .Include(bp => bp.BatchPlanningInstructors)
                .FirstOrDefaultAsync(bp => bp.BatchPlanningId == id);

            if (batchPlanning == null)
                return NotFound();

            _context.BatchPlanningInstructors.RemoveRange(batchPlanning.BatchPlanningInstructors);
            _context.BatchPlannings.Remove(batchPlanning);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
