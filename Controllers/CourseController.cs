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
    public class CourseController : ControllerBase
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ApplicationDbContext _dbContext;

        public CourseController(IRepository<Course> courseRepository, ApplicationDbContext dbContext)
        {
            _courseRepository = courseRepository;
            _dbContext = dbContext;
        }

        // ✅ Get all courses (with instructors optionally)
        [HttpGet("GetCourses")]
        public async Task<ActionResult<IEnumerable<object>>> GetCourses()
        {
            var courses = await _dbContext.Courses
                .Include(c => c.InstructorCourse_Junction_Tables)
                    .ThenInclude(ic => ic.Instructor)
                    .ThenInclude(i => i.Employee)
                    .Include(clr => clr.ClassRoomCourse_Junction_Tables)
                .ThenInclude(cr => cr.ClassRoom)

                .ToListAsync();

            var result = courses.Select(c => new
            {
                c.CourseId,
                c.CourseName,
                c.ShortCode,
                c.TotalHours,
                c.CourseFee,
                c.Remarks,
                c.IsActive,
                c.CreatedDate,
                Instructors = c.InstructorCourse_Junction_Tables.Select(ic => new
                {
                    ic.InstructorId,
                    ic.Instructor.Employee?.EmployeeName,
                    ic.AssignmentDate,
                    ic.IsPrimaryInstructor
                }).ToList(),
                ClassRoom = c.ClassRoomCourse_Junction_Tables.Select(cr => new
                {
                    ClassRoomId = cr.ClassRoomId,
                    RoomName = cr.ClassRoom.RoomName,
                    CourseName = cr.Course.CourseName,
                    IsAvailable = cr.IsAvailable
                }).ToList()
            });

            return Ok(result);
        }

        // ✅ Get specific course with assigned instructors
        [HttpGet("GetCourse/{id}")]
        public async Task<ActionResult<object>> GetCourse(int id)
        {
            var course = await _dbContext.Courses
                .Include(c => c.InstructorCourse_Junction_Tables)
                    .ThenInclude(ic => ic.Instructor)
                        .ThenInclude(i => i.Employee)
                .Include(clr => clr.ClassRoomCourse_Junction_Tables)
                .ThenInclude(cr => cr.ClassRoom)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            return Ok(new
            {
                course.CourseId,
                course.CourseName,
                course.ShortCode,
                course.TotalHours,
                course.CourseFee,
                course.Remarks,
                course.IsActive,
                course.CreatedDate,
                Instructors = course.InstructorCourse_Junction_Tables.Select(ic => new
                {
                    InstructorId = ic.InstructorId,
                    EmployeeName = ic.Instructor.Employee?.EmployeeName,
                    IsPrimaryInstructor = ic.IsPrimaryInstructor,
                    AssignmentDate = ic.AssignmentDate
                }).ToList(),
                   ClassRooms = course.ClassRoomCourse_Junction_Tables.Select(cr => new
                   {
                       ClassRoomId = cr.ClassRoomId,
                       RoomName = cr.ClassRoom.RoomName,
                       CourseName = cr.Course.CourseName,
                       IsAvailable = cr.IsAvailable
                   }).ToList()
            });
        }

        // ✅ Insert course + assign instructors via InstructorCourse_Junction_Table
        [HttpPost("InsertCourse")]
        public async Task<ActionResult<Course>> CreateCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            course.CreatedDate = DateTime.Now;

            var assignedInstructors = course.InstructorCourse_Junction_Tables?.ToList() ?? new();
            var assignedClassRooms = course.ClassRoomCourse_Junction_Tables?.ToList() ?? new();

            course.InstructorCourse_Junction_Tables = null;
            course.ClassRoomCourse_Junction_Tables = null;

            await _courseRepository.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            foreach (var instructorCourse in assignedInstructors)
            {
                instructorCourse.CourseId = course.CourseId;
                instructorCourse.AssignmentDate = DateTime.Now;
                _dbContext.InstructorCourse_Junction_Tables.Add(instructorCourse);
            }

            foreach (var room in assignedClassRooms)
            {
                room.CourseId = course.CourseId;
                _dbContext.ClassRoomCourse_Junction_Tables.Add(room);
            }

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        // ✅ Update course info + reassign instructors and classrooms
        [HttpPut("UpdateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course updatedCourse)
        {
            if (id != updatedCourse.CourseId) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingCourse = await _dbContext.Courses
                .Include(c => c.InstructorCourse_Junction_Tables)
                .Include(c => c.ClassRoomCourse_Junction_Tables)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (existingCourse == null) return NotFound();

            // Update main course properties
            existingCourse.CourseName = updatedCourse.CourseName;
            existingCourse.ShortCode = updatedCourse.ShortCode;
            existingCourse.TotalHours = updatedCourse.TotalHours;
            existingCourse.CourseFee = updatedCourse.CourseFee;
            existingCourse.Remarks = updatedCourse.Remarks;
            existingCourse.IsActive = updatedCourse.IsActive;

            // Remove old assignments
            _dbContext.InstructorCourse_Junction_Tables.RemoveRange(existingCourse.InstructorCourse_Junction_Tables);
            _dbContext.ClassRoomCourse_Junction_Tables.RemoveRange(existingCourse.ClassRoomCourse_Junction_Tables);

            // Add new assignments
            foreach (var instructorCourse in updatedCourse.InstructorCourse_Junction_Tables ?? new List<InstructorCourse_Junction_Table>())
            {
                instructorCourse.CourseId = id;
                instructorCourse.AssignmentDate = DateTime.Now;
                _dbContext.InstructorCourse_Junction_Tables.Add(instructorCourse);
            }

            foreach (var room in updatedCourse.ClassRoomCourse_Junction_Tables ?? new List<ClassRoomCourse_Junction_Table>())
            {
                room.CourseId = id;
                _dbContext.ClassRoomCourse_Junction_Tables.Add(room);
            }

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete course
        [HttpDelete("DeleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _dbContext.Courses
                .Include(c => c.InstructorCourse_Junction_Tables)
                .Include(c => c.ClassRoomCourse_Junction_Tables)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            // Remove related assignments
            _dbContext.InstructorCourse_Junction_Tables.RemoveRange(course.InstructorCourse_Junction_Tables);
            _dbContext.ClassRoomCourse_Junction_Tables.RemoveRange(course.ClassRoomCourse_Junction_Tables);

            await _courseRepository.DeleteAsync(course);
            return NoContent();
        }

        // ✅ Get only active courses
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCourses()
        {
            var activeCourses = await _dbContext.Courses
                .Where(c => c.IsActive)
                .Include(c => c.InstructorCourse_Junction_Tables)
                    .ThenInclude(ic => ic.Instructor)
                        .ThenInclude(i => i.Employee)
                .ToListAsync();

            var result = activeCourses.Select(c => new
            {
                c.CourseId,
                c.CourseName,
                Instructors = c.InstructorCourse_Junction_Tables.Select(ic => ic.Instructor.Employee?.EmployeeName)
            });

            return Ok(result);
        }

        // ✅ Get course with assigned classrooms
        [HttpGet("GetCourseWithClassRooms/{id}")]
        public async Task<ActionResult<object>> GetCourseWithClassRooms(int id)
        {
            var course = await _dbContext.Courses
                .Include(c => c.ClassRoomCourse_Junction_Tables)
                    .ThenInclude(cc => cc.ClassRoom)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            return Ok(new
            {
                course.CourseId,
                course.CourseName,
                ClassRooms = course.ClassRoomCourse_Junction_Tables.Select(cc => new
                {
                    cc.ClassRoomId,
                    cc.ClassRoom.RoomName,
                    cc.IsAvailable
                }).ToList()
            });
        }

        
    }
}
