using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchTransferController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BatchTransferController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BatchTransfers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BatchTransfer>>> GetBatchTransfers()
        {
            return await _context.BatchTransfers
                .Include(bt => bt.Trainee)
                .Include(bt => bt.Batch)
                .OrderBy(t => t.TransferDate.HasValue) // NULLs first
        .ThenBy(t => t.CreatedDate)           // Then by date if exists
        .ThenBy(t => t.TransferDate)
                .ToListAsync();
        }

        // GET: api/BatchTransfers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BatchTransfer>> GetBatchTransfer(int id)
        {
            var batchTransfer = await _context.BatchTransfers
                .Include(bt => bt.Trainee)
                .Include(bt => bt.Batch)
                .FirstOrDefaultAsync(bt => bt.TraineeId == id);

            if (batchTransfer == null)
            {
                return NotFound();
            }

            return batchTransfer;
        }

        //[HttpPost]
        //public async Task<ActionResult<BatchTransfer>> PostBatchTransfer(BatchTransfer batchTransfer)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (!_context.Trainees.Any(t => t.TraineeId == batchTransfer.TraineeId))
        //    {
        //        return BadRequest("Trainee not found");
        //    }

        //    if (!_context.Batchs.Any(v => v.BatchId == batchTransfer.BatchId))
        //    {
        //        return BadRequest("Batch not found");
        //    }

        //    // Removed the automatic CreatedDate setting

        //    _context.BatchTransfers.Add(batchTransfer);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (BatchTransferExists(batchTransfer.TraineeId, batchTransfer.BatchId))
        //        {
        //            return Conflict("This batch transfer already exists");
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetBatchTransfer", new { id = batchTransfer.TraineeId }, batchTransfer);
        //}       

        [HttpPost]
        public async Task<ActionResult<BatchTransfer>> PostBatchTransfer(BatchTransfer batchTransfer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_context.Trainees.Any(t => t.TraineeId == batchTransfer.TraineeId))
            {
                return BadRequest("Trainee not found");
            }

            if (!_context.Batches.Any(v => v.BatchId == batchTransfer.BatchId))
            {
                return BadRequest("Batch not found");
            }

            // Ensure CreatedDate is always null for new entries
            batchTransfer.CreatedDate = null;

            _context.BatchTransfers.Add(batchTransfer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BatchTransferExists(batchTransfer.TraineeId, batchTransfer.BatchId))
                {
                    return Conflict("This batch transfer already exists");
                }
                throw;
            }

            return CreatedAtAction("GetBatchTransfer", new { id = batchTransfer.TraineeId }, batchTransfer);
        }


        private bool BatchTransferExists(int traineeId, int batchId)
        {
            return _context.BatchTransfers.Any(e => e.TraineeId == traineeId && e.BatchId == batchId);
        }       

    }
}