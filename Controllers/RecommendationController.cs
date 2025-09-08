using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;
using TrainingCenter_Api.Models.DTOs;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecommendationController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        [HttpGet("GetRecommendations")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecommendations()
        {
            var data = await _context.Recommendations
                .Include(r => r.Trainee)
                    .ThenInclude(t => t.Registration)
                .Include(r => r.Batch)
                .Include(r => r.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(r => r.Assessment)
                .Include(r => r.Invoice)
                .Select(r => new
                {
                    r.RecommendationId,
                    TraineeName = r.Trainee != null && r.Trainee.Registration != null
        ? r.Trainee.Registration.TraineeName
        : "Unnamed Trainee",
                    TraineeIDNo = r.Trainee != null ? r.Trainee.TraineeIDNo : null,   // এই লাইনটা যোগ করো
                    InstructorName = r.Instructor != null && r.Instructor.Employee != null
        ? r.Instructor.Employee.EmployeeName
        : null,
                    BatchName = r.Batch != null ? r.Batch.BatchName : null,
                    r.RecommendationStatus,
                    r.RecommendationDate,
                    r.AssessmentId,
                    r.InvoiceId,
                    r.RecommendationText,
                })

                .ToListAsync();

            return Ok(data);
        }



        [HttpGet("GetRecommendation/{id}")]
        public async Task<ActionResult<object>> GetRecommendation(int id)
        {
            var recommendation = await _context.Recommendations
                .Include(r => r.Trainee)
                    .ThenInclude(t => t.Registration)
                .Include(r => r.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(r => r.Batch)
                .Include(r => r.Assessment)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.RecommendationId == id);

            if (recommendation == null)
            {
                return NotFound();
            }

            // Create a custom response object that includes TraineeIdNo
            var response = new
            {
                // Recommendation properties
                recommendation.RecommendationId,
                recommendation.RecommendationDate,
                recommendation.RecommendationText,
                recommendation.RecommendationStatus,
                recommendation.BatchId,
                recommendation.TraineeId,
                recommendation.InstructorId,
                recommendation.AssessmentId,
                recommendation.InvoiceId,

                // Navigation properties
                Batch = recommendation.Batch != null ? new
                {
                    recommendation.Batch.BatchId,
                    recommendation.Batch.BatchName
                } : null,

                Trainee = recommendation.Trainee != null ? new
                {
                    recommendation.Trainee.TraineeId,
                    TraineeIDNo = recommendation.Trainee.TraineeIDNo,
                    Registration = recommendation.Trainee.Registration != null ? new
                    {
                        recommendation.Trainee.Registration.TraineeName
                    } : null
                } : null,

                Instructor = recommendation.Instructor != null ? new
                {
                    recommendation.Instructor.InstructorId,
                    Employee = recommendation.Instructor.Employee != null ? new
                    {
                        recommendation.Instructor.Employee.EmployeeName
                    } : null
                } : null,

                Assessment = recommendation.Assessment,
                Invoice = recommendation.Invoice
            };

            return Ok(response);
        }

        [HttpPost("InsertRecommendation")]
        public async Task<IActionResult> InsertRecommendation([FromBody] RecommendationCreateDTO dto)
        {
            if (dto == null || dto.Recommendations == null || dto.Recommendations.Count == 0)
                return BadRequest("Invalid data.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var detail in dto.Recommendations)
                {
                    var recommendation = new Recommendation
                    {
                        RecommendationDate = dto.RecommendationDate,
                        BatchId = dto.BatchId,
                        InstructorId = dto.InstructorId,
                        TraineeId = detail.TraineeId,
                        AssessmentId = detail.AssessmentId,
                        InvoiceId = detail.InvoiceId,
                        RecommendationText = detail.RecommendationText,
                        RecommendationStatus = detail.RecommendationStatus
                    };

                    _context.Recommendations.Add(recommendation);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Recommendations saved successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        // ================= PUT ========================
        [HttpPut("UpdateRecommendation/{id}")]
        public async Task<IActionResult> UpdateRecommendation(int id, [FromBody] RecommendationDTO dto)
        {
            if (id != dto.RecommendationId)
                return BadRequest("ID mismatch.");

            try
            {
                var recommendation = await _context.Recommendations
                    .Include(r => r.Assessment)
                    .Include(r => r.Invoice)
                    .FirstOrDefaultAsync(r => r.RecommendationId == id);

                if (recommendation == null)
                    return NotFound();

                // Update fields
                recommendation.RecommendationText = dto.RecommendationText;
                recommendation.RecommendationStatus = dto.RecommendationStatus;
                recommendation.RecommendationDate = dto.RecommendationDate;
                recommendation.InstructorId = dto.InstructorId;
                recommendation.TraineeId = dto.TraineeId;
                recommendation.BatchId = dto.BatchId;

                _context.Recommendations.Update(recommendation);
                await _context.SaveChangesAsync();

                return Ok(recommendation);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // DELETE: api/Recommendation/5
        [HttpDelete("DeleteRecommendation/{id}")]
        public async Task<IActionResult> DeleteRecommendation(int id)
        {
            var recommendation = await _context.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                return NotFound();
            }

            _context.Recommendations.Remove(recommendation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetInsTraiByBatch/{batchId}")]
        public async Task<ActionResult<object>> GetBatchWithDetails(int batchId)
        {


            // First get batch with instructor info
            var batch = await _context.Batches
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(b => b.BatchId == batchId);

            if (batch == null)
            {
                return NotFound();
            }

            // Then get all trainees for this batch
            var trainees = await _context.Trainees
                .Where(t => t.BatchId == batchId)
                .Include(t => t.Registration)
                .ToListAsync();

            var result = new
            {
                Instructor = batch.Instructor != null ? new
                {
                    InstructorId = batch.Instructor.InstructorId,
                    InstructorName = batch.Instructor.Employee?.EmployeeName
                } : null,

                Trainees = trainees.Select(t => new
                {
                    TraineeId = t.TraineeId,
                    TraineeName = t.Registration?.TraineeName
                }).ToList()
            };

            return Ok(result);
        }


        

        [HttpGet("GetInvAssessbyTrainee/{traineeId}")]
        public async Task<ActionResult<object>> GetInvAssessbyTrainee(int traineeId)
        {
            // ১. Trainee থেকে AdmissionId বের করা
            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.TraineeId == traineeId);

            if (trainee == null)
            {
                return NotFound("Trainee not found");
            }

            // ২. AdmissionId দিয়ে MoneyReceipts নিয়ে আসা + Invoice navigation eager load
            var moneyReceipts = await _context.MoneyReceipts
                .Where(mr => mr.AdmissionId == trainee.AdmissionId)
                .Include(mr => mr.Invoice)
                .ToListAsync();

            // ৩. Invoice গুলো distinct করা (InvoiceNo ভিত্তিতে)
            var distinctInvoices = moneyReceipts
                .Where(mr => mr.Invoice != null) // Invoice নাল নয়
                .GroupBy(mr => mr.Invoice!.InvoiceNo)
                .Select(g => g.First().Invoice)  // প্রথম Invoice নিয়ে আসা
                .ToList();

            // ৪. Assessment গুলো TraineeId দিয়ে filter করা
            var assessments = await _context.Assessments
                .Where(a => a.TraineeId == traineeId)
                .ToListAsync();

            return Ok(new
            {
                invoices = distinctInvoices.Select(inv => new
                {
                    inv!.InvoiceId,
                    inv.InvoiceNo,
                    inv.CreatingDate,
                    inv.InvoiceCategory,
                    inv.MoneyReceiptNo
                }),
                assessments = assessments.Select(a => new
                {
                    a.AssessmentId,
                    a.AssessmentType,
                    a.AssessmentDate,
                    a.OverallScore,
                    a.IsFinalized,
                    a.TheoreticalScore,
                    a.PracticalScore
                }),
                traineeName = trainee.Registration?.TraineeName // Registration থেকে নাম
            });
        }


        [HttpGet("trainee-payment-summary/{traineeId}")]
        public async Task<IActionResult> GetTraineePaymentSummary(int traineeId)
        {
            try
            {
                var trainee = await _context.Trainees
                    .Include(t => t.Registration)
                    .Include(t => t.Admission)
                        .ThenInclude(a => a.AdmissionDetails)
                            .ThenInclude(ad => ad.Batch)
                                .ThenInclude(b => b.Course)
                    .Include(t => t.Admission)
                        .ThenInclude(a => a.Offer)
                    .FirstOrDefaultAsync(t => t.TraineeId == traineeId);

                if (trainee == null)
                {
                    return NotFound("Trainee not found");
                }

                // Get the visitor ID from the registration
                var visitorId = trainee.Registration?.VisitorId;
                if (visitorId == null)
                {
                    return NotFound("Visitor information not found");
                }

                // 1. Calculate total course fees after discounts
                decimal rawTotal = trainee.Admission?.AdmissionDetails
                    .Sum(ad => ad.Batch?.Course?.CourseFee ?? 0) ?? 0;

                decimal offerDiscount = trainee.Admission?.Offer != null
                    ? rawTotal * (trainee.Admission.Offer.DiscountPercentage / 100)
                    : 0;

                decimal totalAfterDiscount = rawTotal - offerDiscount - (trainee.Admission?.DiscountAmount ?? 0);

                // 2. Get ALL payments for this visitor (both Course and Registration Fee)
                decimal totalPaid = await _context.MoneyReceipts
                    .Where(mr => mr.VisitorId == visitorId)
                    .SumAsync(mr => mr.PaidAmount);

                // 3. Calculate due amount
                decimal dueAmount = totalAfterDiscount - totalPaid;
                if (dueAmount < 0) dueAmount = 0;

                string statusMessage = dueAmount == 0 ? "Cleared" : "Not Cleared";

                // 4. Get invoice information if exists
                var invoice = await _context.MoneyReceipts
                    .Where(mr => mr.VisitorId == visitorId && mr.InvoiceId != null)
                    .Select(mr => mr.Invoice)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    InvoiceNo = invoice?.InvoiceNo ?? "Invoice not created",
                    TotalAmount = totalAfterDiscount,
                    TotalPaid = totalPaid,
                    DueAmount = dueAmount,
                    StatusMessage = statusMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // RecommendationController.cs
        [HttpGet("GetRecommendationsByBatch/{batchId}")]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetRecommendationsByBatch(int batchId)
        {
            return await _context.Recommendations
                .Where(r => r.BatchId == batchId)
                .ToListAsync();
        }
    }
}