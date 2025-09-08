using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var schedules = await _context.ClassSchedules
                .Include(cs => cs.Slot)
                .ToListAsync();

            // Populate SelectedDayIds for each schedule
            foreach (var schedule in schedules)
            {
                if (!string.IsNullOrWhiteSpace(schedule.SelectedDays))
                {
                    var dayNames = schedule.SelectedDays
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(d => d.Trim())
                        .ToList();

                    schedule.SelectedDayIds = await _context.Days
                        .Where(d => dayNames.Contains(d.DayName))
                        .Select(d => d.DayId)
                        .ToListAsync();
                }
            }

            return schedules;
        }


        // ✅ GET: Single Schedule
        [HttpGet("GetSchedule/{id}")]
        public async Task<ActionResult<ClassSchedule>> GetSchedule(int id)
        {
            var schedule = await _context.ClassSchedules
                .Include(cs => cs.Slot)
                .FirstOrDefaultAsync(cs => cs.ClassScheduleId == id);

            if (schedule == null)
                return NotFound();

            // Populate SelectedDayIds from SelectedDays string
            if (!string.IsNullOrWhiteSpace(schedule.SelectedDays))
            {
                var dayNames = schedule.SelectedDays
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .ToList();

                schedule.SelectedDayIds = await _context.Days
                    .Where(d => dayNames.Contains(d.DayName))
                    .Select(d => d.DayId)
                    .ToListAsync();
            }

            return schedule;
        }


        // ✅ POST: Insert Single Schedule
        [HttpPost("InsertSchedule")]
        public async Task<ActionResult<ClassSchedule>> InsertSchedule([FromBody] ClassScheduleDto scheduleDto)
        {
            // Create new ClassSchedule from DTO
            var schedule = new ClassSchedule
            {
                SlotId = scheduleDto.SlotId,
                ScheduleDate = scheduleDto.ScheduleDate,
                SelectedDayIds = scheduleDto.SelectedDayIds,
                IsActive = scheduleDto.IsActive
            };

            // Convert SelectedDayIds to SelectedDays string
            if (schedule.SelectedDayIds != null && schedule.SelectedDayIds.Any())
            {
                var dayNames = await _context.Days
                    .Where(d => schedule.SelectedDayIds.Contains(d.DayId))
                    .Select(d => d.DayName)
                    .ToListAsync();

                schedule.SelectedDays = string.Join(", ", dayNames);
            }

            _context.ClassSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.ClassScheduleId }, schedule);
        }


        //✅ PUT: Update Schedule
        [HttpPut("UpdateSchedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ClassScheduleDto scheduleDto)
        {
            var existingSchedule = await _context.ClassSchedules.FindAsync(id);
            if (existingSchedule == null)
                return NotFound();

            existingSchedule.SlotId = scheduleDto.SlotId;
            existingSchedule.ScheduleDate = scheduleDto.ScheduleDate;
            existingSchedule.IsActive = scheduleDto.IsActive;

            if (scheduleDto.SelectedDayIds != null && scheduleDto.SelectedDayIds.Any())
            {
                var dayNames = await _context.Days
                    .Where(d => scheduleDto.SelectedDayIds.Contains(d.DayId))
                    .Select(d => d.DayName)
                    .ToListAsync();

                existingSchedule.SelectedDays = string.Join(", ", dayNames);
            }
            else
            {
                existingSchedule.SelectedDays = null;
            }

            await _context.SaveChangesAsync();
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


        // Add this to your Models folder
        public class ClassScheduleDto
        {
            [Required]
            public int SlotId { get; set; }

            [Required]
            public DateTime ScheduleDate { get; set; }

            public List<int> SelectedDayIds { get; set; } = new List<int>();

            public bool IsActive { get; set; }
        }
    }
}