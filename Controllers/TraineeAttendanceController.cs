using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;
using TrainingCenter_Api.Models.ViewModels;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeAttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TraineeAttendanceController> _logger;
        public TraineeAttendanceController(ApplicationDbContext context, ILogger<TraineeAttendanceController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: api/TraineeAttendance
        [HttpGet("GetTraineeAttendances")]
        public async Task<ActionResult<IEnumerable<TraineeAttendance>>> GetTraineeAttendances()
        {
            return await _context.TraineeAttendances
                .Include(ta => ta.Batch)
                .Include(ta => ta.Instructor)
                .Include(ta => ta.TraineeAttendanceDetails)
                    .ThenInclude(tad => tad.Trainee)
                .ToListAsync();
        }

        
        [HttpGet("GetTraineeAttendance/{id}")]
        public async Task<ActionResult<TraineeAttendance>> GetTraineeAttendance(int id)
        {
            var traineeAttendance = await _context.TraineeAttendances
                .Include(ta => ta.Batch)
                    .ThenInclude(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(ta => ta.TraineeAttendanceDetails)
                    .ThenInclude(tad => tad.Trainee)
                    .ThenInclude(t => t.Registration)
                .Include(ta => ta.TraineeAttendanceDetails)
                    .ThenInclude(tad => tad.Admission)
                .Include(ta => ta.TraineeAttendanceDetails)
                    .ThenInclude(tad => tad.Invoice)
                .Select(ta => new
                {
                    ta.TraineeAttendanceId,
                    ta.AttendanceDate,
                    ta.BatchId,
                    ta.InstructorId,
                    Batch = new
                    {
                        ta.Batch.BatchId,
                        ta.Batch.BatchName,
                        Instructor = new
                        {
                            ta.Batch.Instructor.InstructorId,
                            Employee = new
                            {
                                ta.Batch.Instructor.Employee.EmployeeId,
                                ta.Batch.Instructor.Employee.EmployeeName
                            }
                        }
                    },
                    Instructor = new
                    {
                        ta.Instructor.InstructorId,
                        Employee = new
                        {
                            ta.Instructor.Employee.EmployeeId,
                            ta.Instructor.Employee.EmployeeName
                        }
                    },
                    TraineeAttendanceDetails = ta.TraineeAttendanceDetails.Select(tad => new
                    {
                        tad.TraineeAttendanceDetailId,
                        tad.TraineeId,
                        tad.AdmissionId,
                        tad.InvoiceId,
                        tad.AttendanceStatus,
                        tad.MarkedTime,
                        tad.Remarks,
                        Trainee = new
                        {
                            tad.Trainee.TraineeId,
                            tad.Trainee.TraineeIDNo,
                            Registration = tad.Trainee.Registration != null ? new
                            {
                                tad.Trainee.Registration.TraineeName
                            } : null
                        },
                        Admission = new
                        {
                            tad.Admission.AdmissionNo
                        },
                        Invoice = tad.Invoice != null ? new
                        {
                            tad.Invoice.InvoiceNo
                        } : null
                    })
                })
                .FirstOrDefaultAsync(ta => ta.TraineeAttendanceId == id);

            if (traineeAttendance == null)
            {
                return NotFound();
            }

            return Ok(traineeAttendance);
        }

        // POST: api/TraineeAttendance
        [HttpPost("InsertTraineeAttendance")]
        public async Task<ActionResult<TraineeAttendance>> PostTraineeAttendance(TraineeAttendance traineeAttendance)
        {
            // Set current date if not provided
            if (traineeAttendance.AttendanceDate == default)
            {
                traineeAttendance.AttendanceDate = DateTime.Now;
            }

            // Process attendance details
            if (traineeAttendance.TraineeAttendanceDetails != null)
            {
                foreach (var detail in traineeAttendance.TraineeAttendanceDetails)
                {
                    // Set marked time based on attendance status
                    detail.MarkedTime = detail.AttendanceStatus ? DateTime.Now.ToString("HH:mm:ss") : null;

                    // Validate required fields
                    if (detail.TraineeId == 0 || detail.AdmissionId == 0)
                    {
                        return BadRequest("TraineeId and AdmissionId are required for each attendance detail");
                    }
                }
            }

            _context.TraineeAttendances.Add(traineeAttendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTraineeAttendance", new { id = traineeAttendance.TraineeAttendanceId }, traineeAttendance);
        }

        


        [HttpPut("UpdateTraineeAttendance/{id}")]
        public async Task<IActionResult> PutTraineeAttendance(int id, TraineeAttendance traineeAttendance)
        {
            if (id != traineeAttendance.TraineeAttendanceId)
            {
                return BadRequest("ID mismatch");
            }

            // Get existing attendance with details from database
            var existingAttendance = await _context.TraineeAttendances
                .Include(a => a.TraineeAttendanceDetails)
                .FirstOrDefaultAsync(a => a.TraineeAttendanceId == id);

            if (existingAttendance == null)
            {
                return NotFound();
            }

            // Update root properties
            existingAttendance.AttendanceDate = traineeAttendance.AttendanceDate;
            existingAttendance.BatchId = traineeAttendance.BatchId;
            existingAttendance.InstructorId = traineeAttendance.InstructorId;

            // Process attendance details
            foreach (var detail in traineeAttendance.TraineeAttendanceDetails)
            {
                // Try to find existing detail by ID first
                var existingDetail = existingAttendance.TraineeAttendanceDetails
                    .FirstOrDefault(d => d.TraineeAttendanceDetailId == detail.TraineeAttendanceDetailId);

                if (existingDetail != null)
                {
                    // Update existing detail
                    existingDetail.TraineeId = detail.TraineeId;
                    existingDetail.AdmissionId = detail.AdmissionId;
                    existingDetail.InvoiceId = detail.InvoiceId;
                    existingDetail.AttendanceStatus = detail.AttendanceStatus;
                    existingDetail.Remarks = detail.Remarks;

                    // Handle marked time
                    if (detail.AttendanceStatus && string.IsNullOrEmpty(existingDetail.MarkedTime))
                    {
                        existingDetail.MarkedTime = DateTime.Now.ToString("HH:mm:ss");
                    }
                    else if (!detail.AttendanceStatus)
                    {
                        existingDetail.MarkedTime = null;
                    }
                }
                else
                {
                    // If no existing detail found, try to find by traineeId (for backward compatibility)
                    existingDetail = existingAttendance.TraineeAttendanceDetails
                        .FirstOrDefault(d => d.TraineeId == detail.TraineeId);

                    if (existingDetail != null)
                    {
                        // Update existing detail
                        existingDetail.TraineeId = detail.TraineeId;
                        existingDetail.AdmissionId = detail.AdmissionId;
                        existingDetail.InvoiceId = detail.InvoiceId;
                        existingDetail.AttendanceStatus = detail.AttendanceStatus;
                        existingDetail.Remarks = detail.Remarks;

                        // Handle marked time
                        if (detail.AttendanceStatus && string.IsNullOrEmpty(existingDetail.MarkedTime))
                        {
                            existingDetail.MarkedTime = DateTime.Now.ToString("HH:mm:ss");
                        }
                        else if (!detail.AttendanceStatus)
                        {
                            existingDetail.MarkedTime = null;
                        }
                    }
                    else
                    {
                        // Add new detail if no existing record found
                        detail.TraineeAttendanceId = id;
                        _context.TraineeAttendanceDetails.Add(detail);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TraineeAttendanceExists(id))
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

        // DELETE: api/TraineeAttendance/5
        [HttpDelete("DeleteTraineeAttendance/{id}")]
        public async Task<IActionResult> DeleteTraineeAttendance(int id)
        {
            var traineeAttendance = await _context.TraineeAttendances.FindAsync(id);
            if (traineeAttendance == null)
            {
                return NotFound();
            }

            _context.TraineeAttendances.Remove(traineeAttendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TraineeAttendanceExists(int id)
        {
            return _context.TraineeAttendances.Any(e => e.TraineeAttendanceId == id);
        }



        [HttpGet("GetBatchDetails/{batchId}")]
        public async Task<IActionResult> GetBatchDetails(int batchId)
        {
            try
            {
                var batchDetails = await _context.Batches
                    .Where(b => b.BatchId == batchId)
                    .Select(b => new
                    {
                        BatchId = b.BatchId,
                        BatchName = b.BatchName,
                        InstructorId = b.Instructor.InstructorId,
                        InstructorName = b.Instructor.Employee.EmployeeName,
                        Trainees = b.Trainees.Select(t => new
                        {
                            TraineeId = t.TraineeId,
                            TraineeName = t.Registration.TraineeName,
                            AdmissionId = t.Admission.AdmissionId,
                            AdmissionNo = t.Admission.AdmissionNo,
                            InvoiceNos = t.Admission.moneyReceipts
                                .Where(mr => mr.InvoiceId != null)
                                .Select(mr => new
                                {
                                    InvoiceId = mr.InvoiceId,
                                    InvoiceNo = mr.Invoice.InvoiceNo
                                })
                                .ToList()
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (batchDetails == null)
                {
                    return NotFound();
                }

                return Ok(batchDetails);
            }
            catch (Exception ex)
            {
                // Log the full error
                _logger.LogError(ex, "Error getting batch details");
                return StatusCode(500, new
                {
                    message = "An error occurred while getting batch details",
                    detailedError = ex.Message
                });
            }
        }

    }
}