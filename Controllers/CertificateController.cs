using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public CertificateController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Certificate
        //[HttpGet("GetCertificates")]
        //public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        //{
        //    var certificates = await _context.Certificates
        //        .Include(c => c.Trainee)
        //        .Include(c => c.Course)
        //        .Select(c => new {
        //            c.CertificateId,
        //            c.CertificateNumber,
        //            c.TraineeId,
        //            TraineeName = c.Trainee.Registration.TraineeName,
        //            c.CourseId,
        //            CourseName = c.Course.CourseName
        //        })
        //        .ToListAsync();

        //    return Ok(certificates);
        //}

        [HttpGet("GetCertificates")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var certificates = await _context.Certificates
                .Select(c => new
                {
                    c.CertificateId,
                    c.CertificateNumber,
                    IssueDate = c.IssueDate.ToString("yyyy-MM-dd"),
                    c.TraineeId,
                    TraineeName = c.Trainee.TraineeIDNo + " - " + c.Trainee.Registration.TraineeName,
                    RegistrationNo = c.Registration.RegistrationNo,
                    BatchName = c.Batch.BatchName,
                    CourseName = c.Course.CourseName,
                    RecommendationStatus = c.Recommendation.RecommendationStatus
                })
                .ToListAsync();

            return Ok(certificates);
        }



        [HttpGet("GetCertificate/{id}")]
        public async Task<IActionResult> GetCertificate(int id)
        {
            var certificate = await _context.Certificates
                .Where(c => c.CertificateId == id)
                .Select(c => new
                {
                    c.CertificateId,
                    c.CertificateNumber,
                    IssueDate = c.IssueDate.ToString("yyyy-MM-dd"),
                    TraineeName = c.Trainee.Registration.TraineeName,
                    RegistrationNo = c.Registration.RegistrationNo,
                    BatchName = c.Batch.BatchName,
                    CourseName = c.Course.CourseName,
                    RecommendationStatus = c.Recommendation.RecommendationStatus
                })
                .FirstOrDefaultAsync();

            if (certificate == null)
                return NotFound();

            return Ok(certificate);
        }


        //// POST: api/Certificate
        //[HttpPost("InsertCertificate")]
        //public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        //{
        //    // Auto-generate Certificate Number
        //    certificate.CertificateNumber = await GenerateCertificateNumberAsync();

        //    certificate.IssueDate = DateTime.Now;

        //    _context.Certificates.Add(certificate);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCertificate", new { id = certificate.CertificateId }, certificate);
        //}


        [HttpPost("InsertCertificate")]
        public async Task<IActionResult> CreateCertificate([FromBody] Certificate model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand("sp_CreateCertificate", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TraineeId", model.TraineeId);
                command.Parameters.AddWithValue("@RegistrationId", model.RegistrationId);
                command.Parameters.AddWithValue("@BatchId", model.BatchId);
                command.Parameters.AddWithValue("@CourseId", model.CourseId);
                command.Parameters.AddWithValue("@RecommendationId", model.RecommendationId);

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();

                return Ok(new { CertificateId = Convert.ToInt32(result) });
            }
        }



        // PUT: api/Certificate/5
        [HttpPut("UpdateCertificate/{id}")]
        public async Task<IActionResult> PutCertificate(int id, Certificate certificate)
        {
            if (id != certificate.CertificateId)
                return BadRequest();

            _context.Entry(certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Certificate/5
        [HttpDelete("DeleteCertificate/{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
                return NotFound();

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificates.Any(e => e.CertificateId == id);
        }

        private async Task<string> GenerateCertificateNumberAsync()
        {
            var lastCert = await _context.Certificates
                .OrderByDescending(c => c.CertificateId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastCert != null && !string.IsNullOrEmpty(lastCert.CertificateNumber))
            {
                var parts = lastCert.CertificateNumber.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"CR No-{nextNumber:00}";
        }

        //[HttpGet("GetTraineeInfo/{traineeId}")]
        //public async Task<IActionResult> GetTraineeInfo(int traineeId)
        //{
        //    var traineeInfo = await _context.Trainees
        //        .Where(t => t.TraineeId == traineeId)
        //        .Select(t => new
        //        {
        //            t.TraineeId,
        //            t.TraineeIDNo,
        //            RegistrationNo = t.Registration.RegistrationNo,
        //            BatchName = t.Batch.BatchName,
        //            CourseName = t.Batch.Course.CourseName,
        //            RecommendationStatus = t.Recommendations
        //                .OrderByDescending(r => r.RecommendationDate)
        //                .Select(r => r.RecommendationStatus)
        //                .FirstOrDefault()
        //        })
        //        .FirstOrDefaultAsync();

        //    if (traineeInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(traineeInfo);
        //}

        [HttpGet("GetTraineeInfo/{traineeId}")]
        public async Task<IActionResult> GetTraineeInfo(int traineeId)
        {
            var traineeInfo = await _context.Trainees
                .Where(t => t.TraineeId == traineeId)
                .Select(t => new
                {
                    t.TraineeId,
                    t.TraineeIDNo,
                    RegistrationId = t.RegistrationId,
                    RegistrationNo = t.Registration.RegistrationNo,
                    BatchId = t.BatchId,
                    BatchName = t.Batch.BatchName,
                    CourseId = t.Batch.CourseId,
                    CourseName = t.Batch.Course.CourseName,
                    RecommendationId = t.Recommendations
                        .OrderByDescending(r => r.RecommendationDate)
                        .Select(r => r.RecommendationId)
                        .FirstOrDefault(),
                    RecommendationStatus = t.Recommendations
                        .OrderByDescending(r => r.RecommendationDate)
                        .Select(r => r.RecommendationStatus)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (traineeInfo == null)
                return NotFound();

            return Ok(traineeInfo);
        }



        [HttpGet("GetAllTraineeIdAndNames")]
        public async Task<IActionResult> GetAllTraineeIdAndNames()
        {
            var result = await _context.Trainees
                .Include(t => t.Registration)
                .Select(t => new
                {
                    t.TraineeId,
                    t.TraineeIDNo,
                    t.Registration.TraineeName
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetAvailableTrainees")]
        public async Task<IActionResult> GetAvailableTrainees()
        {
            var traineesWithCertificates = await _context.Certificates
                .Select(c => c.TraineeId)
                .Distinct()
                .ToListAsync();

            var availableTrainees = await _context.Trainees
                .Where(t => !traineesWithCertificates.Contains(t.TraineeId))
                .Include(t => t.Registration)
                .Select(t => new
                {
                    t.TraineeId,
                    t.TraineeIDNo,
                    t.Registration.TraineeName
                })
                .ToListAsync();

            return Ok(availableTrainees);
        }

        
    }
}