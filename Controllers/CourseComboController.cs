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
    public class CourseComboController : ControllerBase
    {
        private readonly IRepository<CourseCombo> _courseComboRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly ApplicationDbContext _context;

        public CourseComboController(
            IRepository<CourseCombo> courseComboRepository,
            IRepository<Course> courseRepository,
            ApplicationDbContext context)
        {
            _courseComboRepository = courseComboRepository;
            _courseRepository = courseRepository;
            _context = context;
        }             


        // GET: api/CourseCombo/GetCourseCombos
        [HttpGet("GetCourseCombos")]
        public async Task<ActionResult<IEnumerable<CourseCombo>>> GetCourseCombos()
        {
            var combos = await _courseComboRepository.GetAllAsync();
            return Ok(combos.Select(c => new {
                c.CourseComboId,
                c.ComboName,
                c.SelectedCourse,
                SelectedCourseIds = c.SelectedCourseIds ?? new List<int>(), // Ensure it's never null
                c.IsActive,
                c.Remarks
            }));
        }

        // GET: api/CourseCombo/GetCourseCombo/5
        [HttpGet("GetCourseCombo/{id}")]
        public async Task<ActionResult<CourseCombo>> GetCourseCombo(int id)
        {
            var courseCombo = await _courseComboRepository.GetByIdAsync(id);
            if (courseCombo == null)
            {
                return NotFound();
            }

            // Ensure SelectedCourseIds is never null
            courseCombo.SelectedCourseIds = courseCombo.SelectedCourseIds ?? new List<int>();
            return Ok(courseCombo);
        }


        [HttpPost("InsertCourseCombo")]
        public async Task<ActionResult<CourseCombo>> PostCourseCombo(CourseCombo courseCombo)
        {
            // ১. কমপক্ষে ২টি কোর্স সিলেক্ট করা হয়েছে কিনা চেক করুন
            if (courseCombo.SelectedCourseIds == null || courseCombo.SelectedCourseIds.Count < 2)
            {
                return BadRequest("At least 2 courses must be selected");
            }

            // ২. কম্বো নাম ইউনিক কিনা চেক করুন
            bool nameExists = await _context.CourseCombos
                .AnyAsync(cc => cc.ComboName == courseCombo.ComboName);

            if (nameExists)
            {
                return Conflict("A combo with this name already exists");
            }

            // ৩. কোর্স নামগুলো স্ট্রিং হিসেবে সেভ করুন
            var courseNames = await _context.Courses
                .Where(c => courseCombo.SelectedCourseIds.Contains(c.CourseId))
                .Select(c => c.CourseName)
                .OrderBy(name => name)
                .ToListAsync();

            courseCombo.SelectedCourse = string.Join(",", courseNames);
            await _courseComboRepository.AddAsync(courseCombo);
            return Ok(courseCombo);
        }

        // PUT: api/CourseCombo/5
        [HttpPut("UpdateCourseCombo/{id}")]
        public async Task<IActionResult> PutCourseCombo(int id, CourseCombo courseCombo)
        {
            if (id != courseCombo.CourseComboId)
            {
                return BadRequest();
            }

            if (courseCombo.SelectedCourseIds != null && courseCombo.SelectedCourseIds.Any())
            {
                var courseNames = await _context.Courses
                    .Where(c => courseCombo.SelectedCourseIds.Contains(c.CourseId))
                    .Select(c => c.CourseName)
                    .ToListAsync();

                courseCombo.SelectedCourse = string.Join(", ", courseNames);
            }

            try
            {
                await _courseComboRepository.UpdateAsync(courseCombo);
            }
            catch
            {
                if (!await _courseComboRepository.ExistsAsync(id))
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

        // DELETE: api/CourseCombo/5
        [HttpDelete("DeleteCourseCombo/{id}")]
        public async Task<IActionResult> DeleteCourseCombo(int id)
        {
            var courseCombo = await _courseComboRepository.GetByIdAsync(id);
            if (courseCombo == null)
            {
                return NotFound();
            }

            await _courseComboRepository.DeleteAsync(courseCombo);

            return NoContent();
        }

        [HttpGet("CheckNameUnique")]
        public async Task<ActionResult<bool>> CheckNameUnique(string name, int id = 0)
        {
            bool exists = await _context.CourseCombos
                .AnyAsync(cc => cc.CourseComboId != id && cc.ComboName == name);
            return Ok(!exists); // true if unique
        }
    }
}