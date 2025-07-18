using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailySalesRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DailySalesRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DailySalesRecords
        [HttpGet("DailySalesRecords")]
        public async Task<ActionResult<IEnumerable<DailySalesRecord>>> GetDailySalesRecords()
        {
            return await _context.DailySalesRecords
                .Include(d => d.Employee)
                .ToListAsync();
        }

        // GET: api/DailySalesRecords/5
        [HttpGet("DailySalesRecords/{id}")]
        public async Task<ActionResult<DailySalesRecord>> GetDailySalesRecord(int id)
        {
            var dailySalesRecord = await _context.DailySalesRecords
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(d => d.DailySalesRecordId == id);

            if (dailySalesRecord == null)
            {
                return NotFound();
            }

            return dailySalesRecord;
        }

        // GET: api/DailySalesRecords/date?date=2023-07-20
        [HttpGet("date")]
        public async Task<ActionResult<IEnumerable<DailySalesRecord>>> GetDailySalesRecordsByDate(DateTime date)
        {
            return await _context.DailySalesRecords
                .Where(d => d.Date.Date == date.Date)
                .Include(d => d.Employee)
                .ToListAsync();
        }

        // GET: api/DailySalesRecords/employee/5
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<DailySalesRecord>>> GetDailySalesRecordsByEmployee(int employeeId)
        {
            return await _context.DailySalesRecords
                .Where(d => d.EmployeeId == employeeId)
                .Include(d => d.Employee)
                .ToListAsync();
        }

        // POST: api/DailySalesRecords
        [HttpPost("InsertDailySalesRecords")]
        public async Task<ActionResult<DailySalesRecord>> PostDailySalesRecord(DailySalesRecord dailySalesRecord)
        {
            // Set current date if not provided
            if (dailySalesRecord.Date == default)
            {
                dailySalesRecord.Date = DateTime.Now;
            }

            _context.DailySalesRecords.Add(dailySalesRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDailySalesRecord", new { id = dailySalesRecord.DailySalesRecordId }, dailySalesRecord);
        }

        // PUT: api/DailySalesRecords/5
        [HttpPut("UpdateDailySalesRecords/{id}")]
        public async Task<IActionResult> PutDailySalesRecord(int id, DailySalesRecord dailySalesRecord)
        {
            if (id != dailySalesRecord.DailySalesRecordId)
            {
                return BadRequest();
            }

            _context.Entry(dailySalesRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DailySalesRecordExists(id))
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

        // DELETE: api/DailySalesRecords/5
        [HttpDelete("DeleteDailySalesRecords/{id}")]
        public async Task<IActionResult> DeleteDailySalesRecord(int id)
        {
            var dailySalesRecord = await _context.DailySalesRecords.FindAsync(id);
            if (dailySalesRecord == null)
            {
                return NotFound();
            }

            _context.DailySalesRecords.Remove(dailySalesRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/DailySalesRecords/summary/2023/7
        [HttpGet("summary/{year}/{month}")]
        public async Task<ActionResult<object>> GetMonthlySummary(int year, int month)
        {
            var records = await _context.DailySalesRecords
                .Where(d => d.Date.Year == year && d.Date.Month == month)
                .ToListAsync();

            var summary = new
            {
                TotalColdCalls = records.Sum(r => r.ColdCallsMade),
                TotalMeetingsConducted = records.Sum(r => r.MeetingsConducted),
                TotalWalkIns = records.Sum(r => r.WalkInsAttended),
                TotalEnrollments = records.Sum(r => r.Enrollments),
                TotalCollections = records.Sum(r => r.NewCollections),
                TotalDueCollections = records.Sum(r => r.DueCollections)
            };

            return summary;
        }

        private bool DailySalesRecordExists(int id)
        {
            return _context.DailySalesRecords.Any(e => e.DailySalesRecordId == id);
        }

        // GET: api/DailySalesRecord/totalCollection?employeeId=1&year=2025&month=7
        [HttpGet("totalCollection")]
        public async Task<ActionResult<decimal>> GetTotalCollection(int employeeId, int year, int month)
        {
            var total = await _context.DailySalesRecords
                .Where(r => r.EmployeeId == employeeId && r.Date.Year == year && r.Date.Month == month)
                .SumAsync(r => r.NewCollections + r.DueCollections);

            return total;
        }

    }
}