using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssessmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Assessments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assessment>>> GetAssessments()
        {
            return await _context.Assessments
                .Include(a => a.Trainee)
                .Include(a => a.Batch)
                .Include(a => a.Instructor)
                .ToListAsync();
        }

        // GET: api/Assessments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assessment>> GetAssessment(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Trainee)
                .Include(a => a.Batch)
                .Include(a => a.Instructor)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
            {
                return NotFound();
            }

            return assessment;
        }

        // GET: api/Assessments/ByTrainee/5
        [HttpGet("ByTrainee/{traineeId}")]
        public async Task<ActionResult<IEnumerable<Assessment>>> GetAssessmentsByTrainee(int traineeId)
        {
            return await _context.Assessments
                .Include(a => a.Trainee)
                .Include(a => a.Batch)
                .Include(a => a.Instructor)
                .Where(a => a.TraineeId == traineeId)
                .ToListAsync();
        }

        // GET: api/Assessments/ByBatch/5
        [HttpGet("ByBatch/{batchId}")]
        public async Task<ActionResult<IEnumerable<Assessment>>> GetAssessmentsByBatch(int batchId)
        {
            return await _context.Assessments
                .Include(a => a.Trainee)
                .Include(a => a.Batch)
                .Include(a => a.Instructor)
                .Where(a => a.BatchId == batchId)
                .ToListAsync();
        }

        // PUT: api/Assessments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssessment(int id, Assessment assessment)
        {
            if (id != assessment.AssessmentId)
            {
                return BadRequest();
            }

            assessment.LastModifiedDate = DateTime.Now;
            assessment.CalculateOverallScore();

            _context.Entry(assessment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssessmentExists(id))
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

        // POST: api/Assessments
        [HttpPost]
        public async Task<ActionResult<Assessment>> PostAssessment(Assessment assessment)
        {
            assessment.CreatedDate = DateTime.Now;
            assessment.CalculateOverallScore();

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssessment", new { id = assessment.AssessmentId }, assessment);
        }

        // DELETE: api/Assessments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Assessments/Finalize/5
        [HttpPost("Finalize/{id}")]
        public async Task<IActionResult> FinalizeAssessment(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }

            assessment.IsFinalized = true;
            assessment.LastModifiedDate = DateTime.Now;

            _context.Entry(assessment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Assessment finalized successfully." });
        }

        private bool AssessmentExists(int id)
        {
            return _context.Assessments.Any(e => e.AssessmentId == id);
        }
    }
}