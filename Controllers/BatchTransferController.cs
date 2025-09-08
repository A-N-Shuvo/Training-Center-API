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
        [HttpGet("GetBatchTransfers")]
        public async Task<ActionResult<IEnumerable<BatchTransfer_Junction>>> GetBatchTransfers()
        {
            return await _context.batchTransfer_Junctions
                .Include(bt => bt.Trainee)
                 .ThenInclude(t => t.Registration) 
                .Include(bt => bt.Batch)
                .OrderBy(t => t.TransferDate.HasValue) // NULLs first
        .ThenBy(t => t.CreatedDate)           // Then by date if exists
        .ThenBy(t => t.TransferDate)
                .ToListAsync();
        }

        // GET: api/BatchTransfers/5
        [HttpGet("GetBatchTransfer/{id}")]
        public async Task<ActionResult<BatchTransfer_Junction>> GetBatchTransfer(int id)
        {
            var batchTransfer = await _context.batchTransfer_Junctions
                .Include(bt => bt.Trainee)
                .Include(bt => bt.Batch)
                .FirstOrDefaultAsync(bt => bt.TraineeId == id);

            if (batchTransfer == null)
            {
                return NotFound();
            }

            return batchTransfer;
        }



        [HttpPost("InsertBatchTransfer")]
        public async Task<ActionResult<BatchTransfer_Junction>> PostBatchTransfer(BatchTransfer_Junction batchTransfer)
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

            _context.batchTransfer_Junctions.Add(batchTransfer);

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
            return _context.batchTransfer_Junctions.Any(e => e.TraineeId == traineeId && e.BatchId == batchId);
        }

        //[HttpGet("GetTraineeOptions")]
        //public async Task<ActionResult> GetTraineeOptions()
        //{
        //    var trainees = await _context.Trainees
        //        .Include(t => t.Registration)
        //        .Select(t => new
        //        {
        //            TraineeId = t.TraineeId,
        //            TraineeName = t.Registration.TraineeName,
        //            TraineeNo = t.TraineeIDNo
        //        })
        //        .ToListAsync();

        //    return Ok(trainees);
        //}

        [HttpGet("GetTraineeOptions")]
        public async Task<ActionResult> GetTraineeOptions()
        {
            var trainees = await _context.Trainees
                .Include(t => t.Registration)
                .Select(t => new
                {
                    TraineeId = t.TraineeId,
                    DisplayText = t.Registration.TraineeName // 👈 Renamed to match Angular interface
                })
                .ToListAsync();

            return Ok(trainees);
        }


        [HttpGet("GetBatchOptions")]
        public async Task<ActionResult> GetBatchOptions()
        {
            var batches = await _context.Batches
                .Select(b => new
                {
                    BatchId = b.BatchId,
                    BatchName = b.BatchName
                })
                .ToListAsync();

            return Ok(batches);
        }


        [HttpDelete("DeleteBatchTransfer/{id}")]
        public async Task<IActionResult> DeleteBatchTransfer(int id)
        {
            var batchTransfer = await _context.batchTransfer_Junctions.FindAsync(id);
            if (batchTransfer == null)
                return NotFound();

            _context.batchTransfer_Junctions.Remove(batchTransfer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
    }
}