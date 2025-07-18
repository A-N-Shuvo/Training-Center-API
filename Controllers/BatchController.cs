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
    public class BatchController : ControllerBase
    {
        private readonly IRepository<Batch> _batchRepository;
        private readonly ApplicationDbContext _context;

        public BatchController(
            IRepository<Batch> batchRepository,
            ApplicationDbContext context)
        {
            _batchRepository = batchRepository;
            _context = context;
        }

        // ✅ Get Active Batches
        [HttpGet("GetActiveBatches")]
        public async Task<ActionResult<IEnumerable<Batch>>> GetActiveBatches()
        {
            var activeBatches = await _context.Batches
                .Where(b => b.IsActive)
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(b => b.ClassRoom)
                .Include(b => b.ClassSchedules)
                .ToListAsync();

            return Ok(activeBatches);
        }

        // ✅ Get All Batches
        [HttpGet("GetBatches")]
        public async Task<ActionResult<IEnumerable<Batch>>> GetBatches()
        {
            var batches = await _context.Batches
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(b => b.ClassRoom)
                .Include(b => b.ClassSchedules)
                .ToListAsync();

            return Ok(batches);
        }

        // ✅ Get Single Batch
        [HttpGet("GetBatch/{id}")]
        public async Task<ActionResult<Batch>> GetBatch(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                    .ThenInclude(i => i.Employee)
                .Include(b => b.ClassRoom)
                .Include(b => b.ClassSchedules)
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null)
            {
                return NotFound();
            }

            return Ok(batch);
        }

        // ✅ Insert Only Batch (without schedules)
        [HttpPost("InsertBatch")]
        public async Task<ActionResult<Batch>> PostBatch(Batch batch)
        {
            await _batchRepository.AddAsync(batch);
            return CreatedAtAction("GetBatch", new { id = batch.BatchId }, batch);
        }

        // ✅ Insert Batch with ClassSchedules (Master-Detail)
        [HttpPost("InsertBatchWithSchedules")]
        public async Task<ActionResult<Batch>> InsertBatchWithSchedules([FromBody] Batch batch)
        {
            if (batch == null || batch.ClassSchedules == null || !batch.ClassSchedules.Any())
            {
                return BadRequest("Batch and at least one ClassSchedule is required.");
            }

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBatch", new { id = batch.BatchId }, batch);
        }

        // ✅ Update Batch (Without ClassSchedules)
        [HttpPut("UpdateBatch/{id}")]
        public async Task<IActionResult> PutBatch(int id, Batch batch)
        {
            if (id != batch.BatchId)
            {
                return BadRequest();
            }

            try
            {
                await _batchRepository.UpdateAsync(batch);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _batchRepository.ExistsAsync(id))
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

        // ✅ Delete Batch
        [HttpDelete("DeleteBatch/{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null)
            {
                return NotFound();
            }

            await _batchRepository.DeleteAsync(batch);
            return NoContent();
        }
    }
}
