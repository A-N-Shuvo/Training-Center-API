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
    public class AdmissionController : ControllerBase
    {
        private readonly IRepository<Admission> _admissionRepository;
        private readonly IRepository<AdmissionDetail> _admissionDetailRepository;
        private readonly ApplicationDbContext _context;
        public AdmissionController(
            IRepository<Admission> admissionRepository,
            IRepository<AdmissionDetail> admissionDetailRepository,
            ApplicationDbContext context)
        {
            _admissionRepository = admissionRepository;
            _admissionDetailRepository = admissionDetailRepository;
            _context = context;
        }

        // GET: api/Admissions (All Admissions with Details)
        [HttpGet("GetAdmissions")]
        public async Task<ActionResult<IEnumerable<Admission>>> GetAdmissionsWithDetails()
        {
            var admissions = await _admissionRepository.GetAllAsync();
            foreach (var admission in admissions)
            {
                admission.AdmissionDetails = (await _admissionDetailRepository.GetAllAsync())
                    .Where(d => d.AdmissionId == admission.AdmissionId).ToList();
            }
            return Ok(admissions);
        }

        // GET: api/Admissions/5 (Single Admission with Details)
        [HttpGet("GetAdmission/{id}")]
        public async Task<ActionResult<Admission>> GetAdmissionWithDetails(int id)
        {
            var admission = await _admissionRepository.GetByIdAsync(id);
            if (admission == null) return NotFound();

            admission.AdmissionDetails = (await _admissionDetailRepository.GetAllAsync())
                .Where(d => d.AdmissionId == id).ToList();
            return admission;
        }




        [HttpPost("InsertAdmission")]
        public async Task<ActionResult<Admission>> PostAdmissionWithDetails([FromBody] Admission admission)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create new Admission object to avoid tracking issues
                var newAdmission = new Admission
                {
                    VisitorId = admission.VisitorId,
                    OrganizationName = admission.OrganizationName,
                    OfferId = admission.OfferId,
                    DiscountAmount = admission.DiscountAmount,
                    AdmissionDate = admission.AdmissionDate,
                    Remarks = admission.Remarks,
                    AdmissionDetails = new List<AdmissionDetail>() // Initialize empty list
                };

                await _admissionRepository.AddAsync(newAdmission);
                await _context.SaveChangesAsync();

                // 2. Generate AdmissionNo
                newAdmission.AdmissionNo = $"ADM-{DateTime.Now.Year}{newAdmission.AdmissionId.ToString().PadLeft(4, '0')}";
                await _admissionRepository.UpdateAsync(newAdmission);
                await _context.SaveChangesAsync();

                // 3. Process AdmissionDetails
                if (admission.AdmissionDetails?.Any() == true)
                {
                    foreach (var detail in admission.AdmissionDetails)
                    {
                        // Create new AdmissionDetail
                        var newDetail = new AdmissionDetail
                        {
                            AdmissionId = newAdmission.AdmissionId,
                            RegistrationId = detail.RegistrationId,
                            BatchId = detail.BatchId
                        };

                        await _admissionDetailRepository.AddAsync(newDetail);
                        await _context.SaveChangesAsync();

                        // Handle Trainee creation/update
                        var trainee = await _context.Trainees
                            .FirstOrDefaultAsync(t => t.RegistrationId == detail.RegistrationId);

                        if (trainee == null)
                        {
                            var lastNo = await _context.Trainees
                                .OrderByDescending(t => t.TraineeIDNo)
                                .Select(t => t.TraineeIDNo)
                                .FirstOrDefaultAsync();

                            var newNo = lastNo == null ? $"{DateTime.Now.Year}00001"
                                : (long.Parse(lastNo) + 1).ToString();

                            trainee = new Trainee
                            {
                                TraineeIDNo = newNo,
                                RegistrationId = detail.RegistrationId,
                                BatchId = detail.BatchId,
                                AdmissionId = newAdmission.AdmissionId
                            };
                            await _context.Trainees.AddAsync(trainee);
                            await _context.SaveChangesAsync();

                            // Add to BatchTransfer_Junction for the first time
                            var batchTransfer = new BatchTransfer_Junction
                            {
                                TraineeId = trainee.TraineeId,
                                BatchId = detail.BatchId,
                                CreatedDate = DateOnly.FromDateTime(DateTime.Now)

                            };
                            await _context.batchTransfer_Junctions.AddAsync(batchTransfer);
                        }
                        else
                        {
                            // If trainee exists, check if batch is being changed
                            if (trainee.BatchId != detail.BatchId)
                            {
                                // Add to BatchTransfer_Junction for batch transfer
                                var batchTransfer = new BatchTransfer_Junction
                                {
                                    TraineeId = trainee.TraineeId,
                                    BatchId = detail.BatchId,
                                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                                    TransferDate = DateOnly.FromDateTime(DateTime.Now)
                                };
                                await _context.batchTransfer_Junctions.AddAsync(batchTransfer);
                            }

                            trainee.BatchId = detail.BatchId;
                            trainee.AdmissionId = newAdmission.AdmissionId;
                            _context.Trainees.Update(trainee);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                // Reload the admission with details
                var result = await _context.Admissions
                    .Include(a => a.AdmissionDetails)
                    .FirstOrDefaultAsync(a => a.AdmissionId == newAdmission.AdmissionId);

                return CreatedAtAction(nameof(GetAdmissionWithDetails),
                    new { id = newAdmission.AdmissionId }, result);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    Message = "Admission creation failed",
                    Error = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }



        [HttpPut("UpdateAdmission/{id}")]
        public async Task<IActionResult> PutAdmissionWithDetails(int id, [FromBody] Admission admission)
        {
            if (id != admission.AdmissionId)
            {
                return BadRequest("ID mismatch between route and body");
            }

            try
            {
                // 1. First get the existing admission to preserve important fields
                var existingAdmission = await _admissionRepository.GetByIdAsync(id);
                if (existingAdmission == null)
                {
                    return NotFound();
                }


                // 2. Update the fields that can change
                existingAdmission.AdmissionId = id;
                existingAdmission.VisitorId = admission.VisitorId;
                existingAdmission.OrganizationName = admission.OrganizationName;
                existingAdmission.OfferId = admission.OfferId;
                existingAdmission.DiscountAmount = admission.DiscountAmount;
                existingAdmission.Remarks = admission.Remarks;
                existingAdmission.AdmissionDate = admission.AdmissionDate;

                // Preserve the AdmissionNo from the existing record
                admission.AdmissionNo = existingAdmission.AdmissionNo;

                await _admissionRepository.UpdateAsync(existingAdmission);

                // 3. Handle the details separately
                if (admission.AdmissionDetails != null)
                {
                    // Get existing details
                    var existingDetails = (await _admissionDetailRepository.GetAllAsync())
                        .Where(d => d.AdmissionId == id).ToList();

                    // Delete existing details
                    foreach (var detail in existingDetails)
                    {
                        await _admissionDetailRepository.DeleteAsync(detail);
                    }

                    // Add new details
                    foreach (var detail in admission.AdmissionDetails)
                    {
                        var newDetail = new AdmissionDetail
                        {
                            AdmissionId = id,
                            RegistrationId = detail.RegistrationId,
                            BatchId = detail.BatchId
                        };
                        await _admissionDetailRepository.AddAsync(newDetail);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to update admission",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
            }
        }


        // DELETE: api/Admissions/5 (Delete Admission with Details)
        [HttpDelete("DeleteAdmission/{id}")]
        public async Task<IActionResult> DeleteAdmissionWithDetails(int id)
        {
            var admission = await _admissionRepository.GetByIdAsync(id);
            if (admission == null) return NotFound();

            // Delete details first
            var details = (await _admissionDetailRepository.GetAllAsync())
                .Where(d => d.AdmissionId == id).ToList();
            foreach (var detail in details)
            {
                await _admissionDetailRepository.DeleteAsync(detail);
            }

            // Then delete admission
            await _admissionRepository.DeleteAsync(admission);
            return NoContent();
        }
        [HttpGet("trainee-display-list")]
        public async Task<IActionResult> GetTraineeDisplayList()
        {
            var list = await _context.Trainees
                .Include(t => t.Registration)
                .Select(t => new
                {
                    traineeId = t.TraineeId,
                    displayText = t.Registration.TraineeName + " - " + t.TraineeIDNo
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("by-visitor/{visitorId}")]
        public async Task<ActionResult<IEnumerable<Admission>>> GetAdmissionsByVisitor(int visitorId)
        {
            return await _context.Admissions
                .Where(a => a.VisitorId == visitorId)
                .Include(a => a.AdmissionDetails)
                .ThenInclude(ad => ad.Batch)
                .ThenInclude(b => b.Course)
                .ToListAsync();
        }
    }


    //[HttpGet("GetAdmissionsByVisitor/{visitorId}")]
    //    public async Task<ActionResult<IEnumerable<Admission>>> GetAdmissionsByVisitor(int visitorId)
    //    {
    //        var admissions = await _context.Admissions
    //            .Include(a => a.Visitors)
    //            .Include(a => a.Offer)
    //            .Include(a => a.AdmissionNo)
    //            .Where(a => a.VisitorId == visitorId)
    //            .ToListAsync();

    //        if (admissions == null || !admissions.Any())
    //        {
    //            return NotFound();
    //        }

    //        return admissions;
    //    }
}