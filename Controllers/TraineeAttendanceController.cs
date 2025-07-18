using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeAttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TraineeAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TraineeAttendance/ByBatch/5?date=2023-05-20
        [HttpGet("ByBatch/{batchId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAttendanceByBatch(int batchId, [FromQuery] DateTime? date)
        {
            var queryDate = date ?? DateTime.Today;

            return await _context.TraineeAttendances
                .Include(ta => ta.Trainee)
                    .ThenInclude(t => t.Registration)
                .Include(ta => ta.Batch)
                    .ThenInclude(b => b.Instructor)
                        .ThenInclude(i => i.Employee)
                .Where(ta => ta.BatchId == batchId && ta.AttendanceDate.Date == queryDate.Date)
                .Select(ta => new
                {
                    ta.TraineeAttendanceId,
                    ta.Trainee.TraineeIDNo,
                    TraineeName = ta.Trainee.Registration.TraineeName,
                    BatchName = ta.Batch.BatchName,
                    InstructorName = ta.Batch.Instructor.Employee.EmployeeName,
                    ta.AttendanceDate,
                    ta.Status,
                    ta.InvoiceNo,
                    ta.MarkedTime,
                    ta.Remarks
                })
                .ToListAsync();
        }

        // POST: api/TraineeAttendance/MarkAttendance
        [HttpPost("MarkAttendance")]
        public async Task<ActionResult> MarkAttendance()
        {
            var formData = await Request.ReadFormAsync();

            // Parse basic data
            var batchId = int.Parse(formData["batchId"]);
            var attendanceDate = DateTime.Parse(formData["attendanceDate"]);
            var admissionId = int.Parse(formData["admissionId"]);
            var status = formData["status"];
            var remarks = formData["remarks"];

            // Get batch with instructor
            var batch = await _context.Batches
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(b => b.BatchId == batchId);

            if (batch == null)
                return BadRequest("ব্যাচ খুঁজে পাওয়া যায়নি");

            // Get all trainee IDs from form
            var traineeIds = formData["traineeIds"]
                .ToString()
                .Split(',')
                .Select(int.Parse)
                .ToList();

            // Validate trainees belong to batch
            var invalidTrainees = await _context.Trainees
                .Where(t => traineeIds.Contains(t.TraineeId) && t.BatchId != batchId)
                .AnyAsync();

            if (invalidTrainees)
                return BadRequest("কিছু ট্রেনি এই ব্যাচে নেই");

            // Check invoice status
            var invoiceNo = await _context.MoneyReceipts
                .Where(mr => mr.AdmissionId == admissionId)
                .Select(mr => mr.Invoice.InvoiceNo)
                .FirstOrDefaultAsync() ?? "No Invoice";

            // Mark attendance for all trainees
            foreach (var traineeId in traineeIds)
            {
                var attendance = new TraineeAttendance
                {
                    TraineeId = traineeId,
                    BatchId = batchId,
                    AdmissionId = admissionId,
                    AttendanceDate = attendanceDate,
                    Status = status,
                    InvoiceNo = invoiceNo,
                    Remarks = remarks
                };

                _context.TraineeAttendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Success = true,
                Message = "উপস্থিতি সফলভাবে রেকর্ড করা হয়েছে",
                Instructor = batch.Instructor.Employee.EmployeeName,
                Batch = batch.BatchName,
                Date = attendanceDate.ToString("yyyy-MM-dd")
            });
        }

        // GET: api/TraineeAttendance/ByInstructor/5?fromDate=2023-05-01&toDate=2023-05-31
        [HttpGet("ByInstructor/{instructorId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAttendanceByInstructor(
            int instructorId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var startDate = fromDate ?? DateTime.Today.AddDays(-7);
            var endDate = toDate ?? DateTime.Today;

            return await _context.TraineeAttendances
                .Include(ta => ta.Trainee)
                    .ThenInclude(t => t.Registration)
                .Include(ta => ta.Batch)
                .Where(ta => ta.Batch.InstructorId == instructorId &&
                       ta.AttendanceDate >= startDate &&
                       ta.AttendanceDate <= endDate)
                .Select(ta => new
                {
                    ta.TraineeAttendanceId,
                    ta.Trainee.TraineeIDNo,
                    TraineeName = ta.Trainee.Registration.TraineeName,
                    ta.Batch.BatchName,
                    ta.AttendanceDate,
                    ta.Status,
                    ta.InvoiceNo,
                    ta.MarkedTime,
                    ta.Remarks
                })
                .ToListAsync();
        }

        // DELETE: api/TraineeAttendance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.TraineeAttendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.TraineeAttendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
