using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClassScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClassScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: All Schedules
        [HttpGet("GetSchedules")]
        public async Task<ActionResult<IEnumerable<ClassSchedule>>> GetSchedules()
        {
            return await _context.ClassSchedules
                .Include(cs => cs.Batch)
                .Include(cs => cs.Day)
                .Include(cs => cs.Slot)
                .ToListAsync();
        }

        // ✅ GET: Schedules by BatchId
        [HttpGet("GetSchedulesByBatch/{batchId}")]
        public async Task<ActionResult<IEnumerable<ClassSchedule>>> GetSchedulesByBatch(int batchId)
        {
            return await _context.ClassSchedules
                .Where(cs => cs.BatchId == batchId)
                .Include(cs => cs.Day)
                .Include(cs => cs.Slot)
                .ToListAsync();
        }

        // ✅ GET: Single Schedule
        [HttpGet("GetSchedule/{id}")]
        public async Task<ActionResult<ClassSchedule>> GetSchedule(int id)
        {
            var schedule = await _context.ClassSchedules
                .Include(cs => cs.Batch)
                .Include(cs => cs.Day)
                .Include(cs => cs.Slot)
                .FirstOrDefaultAsync(cs => cs.ScheduleId == id);

            if (schedule == null)
                return NotFound();

            return schedule;
        }

        // ✅ POST: Insert Single Schedule
        [HttpPost("InsertSchedule")]
        public async Task<ActionResult<ClassSchedule>> InsertSchedule([FromBody] ClassSchedule schedule)
        {
            _context.ClassSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.ScheduleId }, schedule);
        }

        // ✅ POST: Insert Multiple Schedules (Bulk Insert)
        [HttpPost("InsertScheduleList")]
        public async Task<IActionResult> InsertScheduleList([FromBody] List<ClassSchedule> schedules)
        {
            if (schedules == null || !schedules.Any())
                return BadRequest("No schedules to insert.");

            await _context.ClassSchedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();

            return Ok(schedules);
        }

        // ✅ PUT: Update Schedule
        [HttpPut("UpdateSchedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ClassSchedule schedule)
        {
            if (id != schedule.ScheduleId)
                return BadRequest();

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ClassSchedules.Any(cs => cs.ScheduleId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: Delete Schedule
        [HttpDelete("DeleteSchedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            _context.ClassSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
