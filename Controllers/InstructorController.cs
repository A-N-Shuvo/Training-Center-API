using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorController : ControllerBase
    {
        private readonly IRepository<Instructor> _instructorRepository;
        private readonly ApplicationDbContext _context;

        public InstructorController(
            IRepository<Instructor> instructorRepository,
            ApplicationDbContext context
        )
        {
            _instructorRepository = instructorRepository;
            _context = context;
        }

        // ✅ GET: All Instructors with Courses & BatchPlannings
        [HttpGet("GetInstructors")]
        public async Task<ActionResult<IEnumerable<object>>> GetInstructors()
        {
            var instructors = await _context.Instructors
                .Include(i => i.Employee)
                .Include(i => i.InstructorCourse_Junction_Tables)
                    .ThenInclude(ic => ic.Course)
                .Include(i => i.BatchPlanningInstructors)
                    .ThenInclude(bpi => bpi.BatchPlanning)
                .ToListAsync();

            var result = instructors.Select(i => new
            {
                i.InstructorId,
                i.EmployeeId,
                EmployeeName = i.Employee?.EmployeeName,
                i.IsActive,
                i.Remarks,
                Courses = i.InstructorCourse_Junction_Tables.Select(ic => new
                {
                    ic.CourseId,
                    ic.Course.CourseName,
                    ic.IsPrimaryInstructor
                }),
                Specialization = string.Join(", ", i.InstructorCourse_Junction_Tables.Select(ic => ic.Course.CourseName)),
                SelectedCourseIds = i.InstructorCourse_Junction_Tables.Select(ic => ic.CourseId).ToList(),
                AssignedBatchPlanningIds = i.BatchPlanningInstructors.Select(bpi => bpi.BatchPlanningId).ToList()
            });

            return Ok(result);
        }

        


        [HttpGet("GetInstructor/{id}")]
        public async Task<ActionResult<object>> GetInstructor(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.Employee)
                .Include(i => i.InstructorCourse_Junction_Tables)
                    .ThenInclude(ic => ic.Course)
                .Include(i => i.BatchPlanningInstructors)
                    .ThenInclude(bpi => bpi.BatchPlanning)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (instructor == null) return NotFound();

            return Ok(new
            {
                instructor.InstructorId,
                instructor.EmployeeId,
                EmployeeName = instructor.Employee?.EmployeeName,
                instructor.IsActive,
                instructor.Remarks,
                SelectedCourseIds = instructor.InstructorCourse_Junction_Tables.Select(ic => ic.CourseId).ToList(),
                Courses = instructor.InstructorCourse_Junction_Tables.Select(ic => new
                {
                    ic.CourseId,
                    CourseName = ic.Course?.CourseName,
                    ic.IsPrimaryInstructor
                }).ToList(),
                AssignedBatchPlanningIds = instructor.BatchPlanningInstructors.Select(bpi => bpi.BatchPlanningId).ToList(),
                AssignedBatchPlannings = instructor.BatchPlanningInstructors.Select(bpi => new
                {
                    bpi.BatchPlanning.BatchPlanningId,
                    bpi.BatchPlanning.Year,
                    bpi.BatchPlanning.StartMonth
                }).ToList()
            });
        }

        // ✅ POST: Insert Instructor with Course and BatchPlanning assignments
        [HttpPost("InsertInstructor")]
        public async Task<ActionResult<Instructor>> PostInstructor(Instructor instructor, [FromQuery] List<int> batchPlanningIds)
        {
            await _instructorRepository.AddAsync(instructor);
            await _context.SaveChangesAsync();

            // Assign Courses
            if (instructor.SelectedCourseIds != null && instructor.SelectedCourseIds.Any())
            {
                foreach (var courseId in instructor.SelectedCourseIds)
                {
                    _context.InstructorCourse_Junction_Tables.Add(new InstructorCourse_Junction_Table
                    {
                        InstructorId = instructor.InstructorId,
                        CourseId = courseId,
                        AssignmentDate = DateTime.Now
                    });
                }
            }

            // Assign BatchPlannings
            if (batchPlanningIds != null && batchPlanningIds.Any())
            {
                foreach (var batchId in batchPlanningIds)
                {
                    _context.BatchPlanningInstructors.Add(new BatchPlanningInstructor
                    {
                        InstructorId = instructor.InstructorId,
                        BatchPlanningId = batchId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInstructor), new { id = instructor.InstructorId }, instructor);
        }

        // ✅ PUT: Update Instructor, Courses, and BatchPlannings
        [HttpPut("UpdateInstructor/{id}")]
        public async Task<IActionResult> PutInstructor(int id, Instructor instructor, [FromQuery] List<int> batchPlanningIds)
        {
            if (id != instructor.InstructorId)
                return BadRequest();

            var existingInstructor = await _context.Instructors
                .Include(i => i.InstructorCourse_Junction_Tables)
                .Include(i => i.BatchPlanningInstructors)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (existingInstructor == null)
                return NotFound();

            // Update basic info
            existingInstructor.EmployeeId = instructor.EmployeeId;
            existingInstructor.Remarks = instructor.Remarks;
            existingInstructor.IsActive = instructor.IsActive;

            // Update Courses
            var existingCourses = _context.InstructorCourse_Junction_Tables.Where(ic => ic.InstructorId == id);
            _context.InstructorCourse_Junction_Tables.RemoveRange(existingCourses);

            if (instructor.SelectedCourseIds != null && instructor.SelectedCourseIds.Any())
            {
                foreach (var courseId in instructor.SelectedCourseIds)
                {
                    _context.InstructorCourse_Junction_Tables.Add(new InstructorCourse_Junction_Table
                    {
                        InstructorId = id,
                        CourseId = courseId,
                        AssignmentDate = DateTime.Now
                    });
                }
            }

            // Update BatchPlannings
            var existingBatches = _context.BatchPlanningInstructors.Where(b => b.InstructorId == id);
            _context.BatchPlanningInstructors.RemoveRange(existingBatches);

            if (batchPlanningIds != null && batchPlanningIds.Any())
            {
                foreach (var batchId in batchPlanningIds)
                {
                    _context.BatchPlanningInstructors.Add(new BatchPlanningInstructor
                    {
                        InstructorId = id,
                        BatchPlanningId = batchId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: Instructor and all Course/Batch links
        [HttpDelete("DeleteInstructor/{id}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            try
            {
                var instructor = await _instructorRepository.GetByIdAsync(id);
                if (instructor == null)
                    return NotFound();

                // Remove Courses
                var assignments = _context.InstructorCourse_Junction_Tables.Where(ic => ic.InstructorId == id);
                _context.InstructorCourse_Junction_Tables.RemoveRange(assignments);

                // Remove BatchPlannings
                var batchAssignments = _context.BatchPlanningInstructors.Where(b => b.InstructorId == id);
                _context.BatchPlanningInstructors.RemoveRange(batchAssignments);

                await _instructorRepository.DeleteAsync(instructor);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Instructor cannot be deleted. It may be referenced elsewhere.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Unexpected error occurred.");
            }
        }

        // ✅ GET: Dropdown list of Instructors with Employee Names
        [HttpGet("GetInstructorsWithEmployees")]
        public async Task<IActionResult> GetInstructorsWithEmployees()
        {
            var instructors = await _context.Instructors
                .Include(i => i.Employee)
                .Select(i => new
                {
                    i.InstructorId,
                    i.EmployeeId,
                    Employee = new
                    {
                        i.Employee.EmployeeId,
                        i.Employee.EmployeeName
                    }
                })
                .ToListAsync();

            return Ok(instructors);
        }
    }
}
