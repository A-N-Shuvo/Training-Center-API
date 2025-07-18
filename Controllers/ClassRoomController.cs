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

    public class ClassRoomController : ControllerBase
    {
        private readonly IRepository<ClassRoom> _classRoomRepository;
        private readonly ApplicationDbContext _dbContext;

        public ClassRoomController(IRepository<ClassRoom> classRoomRepository, ApplicationDbContext dbContext)
        {
            _classRoomRepository = classRoomRepository;
            _dbContext = dbContext;
        }

        // GET: api/ClassRooms
        [HttpGet("GetClassRooms")]
        public async Task<ActionResult<IEnumerable<ClassRoom>>> GetClassRooms()
        {
            var classRooms = await _classRoomRepository.GetAllAsync();
            return Ok(classRooms);
        }

        // GET: api/ClassRooms/5
        [HttpGet("GetClassRoom/{id}")]
        public async Task<ActionResult<ClassRoom>> GetClassRoom(int id)
        {
            var classRoom = await _classRoomRepository.GetByIdAsync(id);

            if (classRoom == null)
            {
                return NotFound();
            }

            return Ok(classRoom);
        }

        // PUT: api/ClassRooms/5
        [HttpPut, Route("UpdateClassRoom/{id}")]
        public async Task<IActionResult> UpdateClassRoom(int id, ClassRoom classRoom)
        {
            if (id != classRoom.ClassRoomId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _classRoomRepository.UpdateAsync(classRoom);
            }
            catch
            {
                if (!await _classRoomRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ClassRooms
        [HttpPost("InsertClassRoom")]
        public async Task<ActionResult<ClassRoom>> PostClassRoom(ClassRoom classRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _classRoomRepository.AddAsync(classRoom);
                return CreatedAtAction("GetClassRoom", new { id = classRoom.ClassRoomId }, classRoom);
            }
            catch
            {
                return StatusCode(500, "An error occurred while creating the classroom.");
            }
        }

        // DELETE: api/ClassRooms/5
        [HttpDelete("DeleteClassRoom/{id}")]
        public async Task<IActionResult> DeleteClassRoom(int id)
        {
            var classRoom = await _classRoomRepository.GetByIdAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }

            try
            {
                await _classRoomRepository.DeleteAsync(classRoom);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the classroom.");
            }
        }

        // ✅ Get classroom with assigned courses
        [HttpGet("GetClassRoomWithCourses/{id}")]
        public async Task<ActionResult<object>> GetClassRoomWithCourses(int id)
        {
            var classRoom = await _classRoomRepository.GetByIdAsync(id);
            if (classRoom == null) return NotFound();

            // Load related manually if using repository
            var assignments = _dbContext.ClassRoomCourse_Junction_Tables
                .Where(cc => cc.ClassRoomId == id)
                .Include(cc => cc.Course)
                .ToList();

            return Ok(new
            {
                classRoom.ClassRoomId,
                classRoom.RoomName,
                Courses = assignments.Select(cc => new
                {
                    cc.CourseId,
                    cc.Course.CourseName,
                    cc.IsAvailable
                }).ToList()
            });
        }

        // ✅ Assign courses to a classroom
        [HttpPost("AssignCoursesToClassRoom")]
        public async Task<IActionResult> AssignCoursesToClassRoom(int classRoomId, [FromBody] List<int> courseIds)
        {
            var classRoom = await _classRoomRepository.GetByIdAsync(classRoomId);
            if (classRoom == null) return NotFound();

            var existingAssignments = _dbContext.ClassRoomCourse_Junction_Tables
                .Where(cc => cc.ClassRoomId == classRoomId)
                .ToList();

            _dbContext.ClassRoomCourse_Junction_Tables.RemoveRange(existingAssignments);

            foreach (var courseId in courseIds)
            {
                _dbContext.ClassRoomCourse_Junction_Tables.Add(new ClassRoomCourse_Junction_Table
                {
                    ClassRoomId = classRoomId,
                    CourseId = courseId,
                    IsAvailable = true
                });
            }

            _dbContext.SaveChangesAsync();
            return Ok("Courses assigned to classroom successfully.");
        }
    }
}