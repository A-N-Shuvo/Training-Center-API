using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Migrations;
using TrainingCenter_Api.Models;
using TrainingCenter_Api.Models.ViewModels;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BatchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BatchController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet("GetBatches")]
        public async Task<ActionResult<IEnumerable<BatchDto>>> GetBatches()
        {
            var batches = await _context.Batches
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(b => b.ClassRoom)
                .AsNoTracking()
                .ToListAsync();

            var batchDtos = batches.Select(b => new BatchDto
            {
                BatchId = b.BatchId,
                BatchName = b.BatchName,
                CourseId = b.CourseId,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                BatchType = b.BatchType,
                InstructorId = b.InstructorId,
                ClassRoomId = b.ClassRoomId,
                IsActive = b.IsActive,
                Remarks = b.Remarks,
                SelectedClassSchedules=b.SelectedClassSchedules,
                PreviousInstructorIdsString=b.PreviousInstructorIdsString,                
                SelectedScheduleIds = !string.IsNullOrEmpty(b.SelectedClassSchedules)
                    ? b.SelectedClassSchedules.Split(',').Select(int.Parse).ToList()
                    : new List<int>(),
                PreviousInstructorIds = !string.IsNullOrEmpty(b.PreviousInstructorIdsString)
                    ? b.PreviousInstructorIdsString.Split(',').Select(int.Parse).ToList()
                    : new List<int>(),
                CourseName = b.Course?.CourseName,
                EmployeeName = b.Instructor?.Employee?.EmployeeName,
                ClassRoomName = b.ClassRoom?.RoomName
            }).ToList();

            return Ok(batchDtos);
        }

        [HttpGet("GetBatch/{id}")]
        public async Task<ActionResult<BatchDto>> GetBatch(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(b => b.ClassRoom)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null)
            {
                return NotFound();
            }

            return Ok(new BatchDto
            {
                BatchId = batch.BatchId,
                BatchName = batch.BatchName,
                CourseId = batch.CourseId,
                StartDate = batch.StartDate,
                EndDate = batch.EndDate,
                BatchType = batch.BatchType,
                InstructorId = batch.InstructorId,
                ClassRoomId = batch.ClassRoomId,
                IsActive = batch.IsActive,
                Remarks = batch.Remarks,
                SelectedClassSchedules = batch.SelectedClassSchedules,
                PreviousInstructorIdsString = batch.PreviousInstructorIdsString,
                SelectedScheduleIds = !string.IsNullOrEmpty(batch.SelectedClassSchedules)
                    ? batch.SelectedClassSchedules.Split(',').Select(int.Parse).ToList()
                    : new List<int>(),
                PreviousInstructorIds = !string.IsNullOrEmpty(batch.PreviousInstructorIdsString)
                    ? batch.PreviousInstructorIdsString.Split(',').Select(int.Parse).ToList()
                    : new List<int>(),
                CourseName = batch.Course?.CourseName,
                EmployeeName = batch.Instructor?.Employee?.EmployeeName,
                ClassRoomName = batch.ClassRoom?.RoomName
            });
        }


        [HttpGet("GetBatches/{id}")]
        public async Task<ActionResult<object>> GetBatches(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(b => b.ClassRoom)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null)
                return NotFound();
            // Fetch previous instructors
            var previousInstructorIds = !string.IsNullOrEmpty(batch.PreviousInstructorIdsString)
                ? batch.PreviousInstructorIdsString.Split(',').Select(int.Parse).ToList()
                : new List<int>();

            var previousInstructors = await _context.Instructors
                .Include(i => i.Employee)
                .Where(i => previousInstructorIds.Contains(i.InstructorId))
                .Select(i => i.Employee.EmployeeName)
                .ToListAsync();


            var selectedScheduleIds = !string.IsNullOrEmpty(batch.SelectedClassSchedules)
                ? batch.SelectedClassSchedules.Split(',').Select(int.Parse).ToList()
                : new List<int>();

            // Load class schedules with Slot
            var classSchedules = await _context.ClassSchedules
                .Include(cs => cs.Slot)
                .Where(cs => selectedScheduleIds.Contains(cs.ClassScheduleId))
                .ToListAsync();

            // Build anonymous objects for schedules
            var scheduleList = classSchedules.Select(cs => new
            {
                classScheduleId = cs.ClassScheduleId,
                selectedDays = cs.SelectedDays,
                slotId = cs.SlotId,
                slot = cs.Slot != null
                    ? new
                    {
                        timeSlotType = cs.Slot.TimeSlotType,
                        startTimeString = cs.Slot.StartTime.ToString("HH:mm"),
                        endTimeString = cs.Slot.EndTime.ToString("HH:mm")
                    }
                    : null,
                scheduleDate = cs.ScheduleDate,
                isActive = cs.IsActive
            }).ToList();

            return Ok(new
            {
                batch.BatchId,
                batch.BatchName,
                batch.CourseId,
                batch.StartDate,
                batch.EndDate,
                batch.BatchType,
                batch.InstructorId,
                batch.ClassRoomId,
                batch.IsActive,
                batch.Remarks,
                batch.SelectedClassSchedules,
                batch.PreviousInstructorIdsString,
                SelectedScheduleIds = selectedScheduleIds,
                PreviousInstructorIds = !string.IsNullOrEmpty(batch.PreviousInstructorIdsString)
                    ? batch.PreviousInstructorIdsString.Split(',').Select(int.Parse).ToList()
                    : new List<int>(),
                CourseName = batch.Course?.CourseName,
                EmployeeName = batch.Instructor?.Employee?.EmployeeName,
                InstructorName = batch.Instructor?.Employee?.EmployeeName,
                ClassRoomName = batch.ClassRoom?.RoomName,
                PreviousInstructorNames = previousInstructors,

                // 🔥 Now schedules included directly
                schedules = scheduleList
            });
        }



        [HttpPost("InsertBatch")]
        public async Task<ActionResult<Batch>> PostBatch(BatchDto batchDto)
        {
            // Get the course first
            var course = await _context.Courses.FindAsync(batchDto.CourseId);
            if (course == null) return BadRequest("Course not found");

            // Find existing batches for this course to determine next number
            var existingBatches = await _context.Batches
                .Where(b => b.CourseId == batchDto.CourseId &&
                           b.BatchName.StartsWith(course.CourseName + "-"))
                .ToListAsync();

            // Determine next batch number
            int nextNumber = 1;
            if (existingBatches.Any())
            {
                var lastNumber = existingBatches
                    .Select(b => {
                        var parts = b.BatchName.Split('-');
                        return parts.Length > 1 ? parts[1] : "0";
                    })
                    .Select(part => int.TryParse(part, out var num) ? num : 0)
                    .Max();

                nextNumber = lastNumber + 1;
            }

            // Create new Batch with auto-generated name
            var batch = new Batch
            {
                BatchName = $"{course.CourseName}-{nextNumber:D2}", // Format as "CourseName-01"
                CourseId = batchDto.CourseId,
                StartDate = batchDto.StartDate,
                EndDate = batchDto.EndDate,
                BatchType = batchDto.BatchType,
                InstructorId = batchDto.InstructorId,
                ClassRoomId = batchDto.ClassRoomId,
                IsActive = batchDto.IsActive,
                Remarks = batchDto.Remarks,
                SelectedClassSchedules = string.Join(",", batchDto.SelectedScheduleIds)
            };

            // Rest of your existing validation and save logic...
            foreach (var id in batchDto.SelectedScheduleIds)
            {
                if (!await _context.ClassSchedules.AnyAsync(s => s.ClassScheduleId == id))
                    return BadRequest($"Schedule with ID {id} not found");
            }

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            return Ok(batch);
        }

        // PUT: api/Batches/5
        [HttpPut("UpdateBatch/{id}")]
        public async Task<IActionResult> PutBatch(int id, BatchDto batchDto)
        {
            if (id != batchDto.BatchId)
            {
                return BadRequest("Batch ID mismatch");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var batch = await _context.Batches.FindAsync(id);
                if (batch == null)
                {
                    return NotFound("Batch not found");
                }

                // Update basic batch properties
                //batch.BatchName = batchDto.BatchName;
                batch.CourseId = batchDto.CourseId;
                batch.StartDate = batchDto.StartDate;
                batch.EndDate = batchDto.EndDate;
                batch.BatchType = batchDto.BatchType;
                batch.InstructorId = batchDto.InstructorId;
                batch.ClassRoomId = batchDto.ClassRoomId;
                batch.IsActive = batchDto.IsActive;
                batch.Remarks = batchDto.Remarks;

                // Convert selected schedule IDs to comma-separated string
                batch.SelectedClassSchedules = batchDto.SelectedScheduleIds != null
                    ? string.Join(",", batchDto.SelectedScheduleIds)
                    : null;

                batch.PreviousInstructorIdsString = batchDto.PreviousInstructorIds != null ?
           string.Join(",", batchDto.PreviousInstructorIds) : null;

                // Verify all referenced schedules exist
                if (batchDto.SelectedScheduleIds != null)
                {
                    foreach (var scheduleId in batchDto.SelectedScheduleIds)
                    {
                        if (!await _context.ClassSchedules.AnyAsync(s => s.ClassScheduleId == scheduleId))
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"ClassSchedule with ID {scheduleId} not found");
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteBatch/{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var batch = await _context.Batches.FindAsync(id);
                if (batch == null)
                {
                    return NotFound("Batch not found");
                }

                // Check if batch has any dependent records
                var hasTrainees = await _context.Trainees.AnyAsync(t => t.BatchId == id);
                var hasAssessments = await _context.Assessments.AnyAsync(a => a.BatchId == id);
                var hasAdmissions = await _context.AdmissionDetails.AnyAsync(ad => ad.BatchId == id);

                if (hasTrainees || hasAssessments || hasAdmissions)
                {
                    return BadRequest("Cannot delete batch because it has related records (trainees, assessments, or admissions).");
                }

                // No need to handle schedule associations since we're just storing IDs
                _context.Batches.Remove(batch);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GenerateBatchName/{courseId}")]
        public async Task<ActionResult<string>> GenerateBatchName(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound("Course not found");

            var lastBatchNumber = await _context.Batches
                .Where(b => b.CourseId == courseId)
                .OrderByDescending(b => b.BatchId)
                .Select(b => b.BatchName)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastBatchNumber != null && lastBatchNumber.Contains("-"))
            {
                var numberPart = lastBatchNumber.Split('-')[1];
                if (int.TryParse(numberPart, out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            return $"{course.CourseName}-{nextNumber:D2}";
        }




        // DTO for Batch
        public class BatchDto
        {
            public int BatchId { get; set; }

            //[Required]
            public string? BatchName { get; set; }

            public string? CourseName { get; set; }

            [Required]
            public int CourseId { get; set; }

            [Required]
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }

            [Required]
            public string BatchType { get; set; }

            [Required]
            public int InstructorId { get; set; }
            public string? EmployeeName { get; set; }
            public List<int> PreviousInstructorIds { get; set; } = new List<int>();
            public string? PreviousInstructorIdsString { get; set; }
            [Required]
            public int ClassRoomId { get; set; }
            public string? ClassRoomName { get; set; }

            public bool IsActive { get; set; } = true;
            public string? Remarks { get; set; }
            public List<int> SelectedScheduleIds { get; set; } = new List<int>();
            public string? SelectedClassSchedules { get; set; }

        }
    }
}