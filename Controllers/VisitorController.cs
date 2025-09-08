using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VisitorController : ControllerBase
    {
        private readonly IRepository<Visitor> _visitorRepository;
        private readonly ApplicationDbContext _context;

        public VisitorController(IRepository<Visitor> visitorRepository, ApplicationDbContext context)
        {
            _visitorRepository = visitorRepository;
            _context = context;
        }
              
        [HttpGet("GetVisitors")]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetVisitors([FromQuery] bool includeEmployee = true)
        {
            if (includeEmployee)
            {
                return Ok(await _context.Visitors.Include(v => v.Employee).ToListAsync());
            }
            return Ok(await _visitorRepository.GetAllAsync());
        }

        [HttpGet("GetVisitor/{id}")]
        public async Task<ActionResult<Visitor>> GetVisitor(int id, [FromQuery] bool includeEmployee = true)
        {
            if (includeEmployee)
            {
                var visitor = await _context.Visitors
                    .Include(v => v.Employee)
                    .FirstOrDefaultAsync(v => v.VisitorId == id);

                if (visitor == null) return NotFound();
                return Ok(visitor);
            }

            var basicVisitor = await _visitorRepository.GetByIdAsync(id);
            if (basicVisitor == null) return NotFound();
            return Ok(basicVisitor);
        }



        [HttpPost("InsertVisitor")]
        public async Task<ActionResult<Visitor>> PostVisitor([FromBody] Visitor visitor)
        {
            if (visitor == null)
                return BadRequest();

            // Generate the next visitor number
            visitor.VisitorNo = await GenerateNextVisitorNumberAsync();

            await _visitorRepository.AddAsync(visitor);

            // ✅ visitor save করার পর VisitorEmployee এ একটি রেকর্ড insert করুন
            var visitorTransfer_Junction = new VisitorTransfer_Junction
            {
                VisitorId = visitor.VisitorId,
                EmployeeId = visitor.EmployeeId,
                CreatedDate = DateTime.Now,
            };

            // আপনি যদি repository pattern ব্যবহার না করেন, তাহলে সরাসরি context ব্যবহার করুন:
            _context.visitorTransfer_Junctions.Add(visitorTransfer_Junction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVisitor), new { id = visitor.VisitorId }, visitor);
        }

        private async Task<string> GenerateNextVisitorNumberAsync()
        {
            // Get the highest current visitor number
            var lastVisitor = await _context.Visitors
                .OrderByDescending(v => v.VisitorId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastVisitor != null && !string.IsNullOrEmpty(lastVisitor.VisitorNo))
            {
                if (int.TryParse(lastVisitor.VisitorNo, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            // Format as 6-digit string with leading zeros
            return nextNumber.ToString("D6");
        }



        [HttpPut, Route("UpdateVisitor/{id}")]
        public async Task<IActionResult> UpdateVisitor(int id, Visitor visitor)
        {
            if (id != visitor.VisitorId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the existing visitor from the database
                var existingVisitor = await _visitorRepository.GetByIdAsync(id);
                if (existingVisitor == null)
                {
                    return NotFound();
                }

                // Preserve the original VisitorNo
                visitor.VisitorNo = existingVisitor.VisitorNo;

                // Update all other properties
                _context.Entry(existingVisitor).CurrentValues.SetValues(visitor);

                // If you're using repository pattern's UpdateAsync, make sure it doesn't overwrite VisitorNo
                await _visitorRepository.UpdateAsync(existingVisitor);
            }
            catch
            {
                if (!await _visitorRepository.ExistsAsync(id))
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
               


        [HttpDelete("DeleteVisitor/{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            try
            {
                var visitor = await _visitorRepository.GetByIdAsync(id);
                if (visitor == null)
                    return NotFound();

                await _visitorRepository.DeleteAsync(visitor);
                return NoContent();
            }
            catch (DbUpdateException ex) when (IsForeignKeyConstraintViolation(ex))
            {
                return BadRequest("Cannot delete this visitor because it has related records in the system. Please delete the related records first.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the visitor.");
            }
        }

        private bool IsForeignKeyConstraintViolation(DbUpdateException ex)
        {
            // Check if this is a SQL Server foreign key violation
            if (ex.InnerException is SqlException sqlException)
            {
                // SQL Server error codes for foreign key violation
                return sqlException.Number == 547;
            }

            // For other databases, you might need different checks
            return false;
        }


        [HttpGet("ByEmployee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetVisitorsByEmployee(int employeeId)
        {
            var visitors = await _context.Visitors
                .Where(v => v.EmployeeId == employeeId)
                .ToListAsync();
            return Ok(visitors);
        }

        

    }
}