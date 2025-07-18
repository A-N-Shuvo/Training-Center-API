using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DesignationController : ControllerBase
    {
        private readonly IRepository<Designation> _designationRepository;

        public DesignationController(IRepository<Designation> designationRepository)
        {
            _designationRepository = designationRepository;
        }

        // GET: api/Designation
        [HttpGet("GetDesignations")]
        public async Task<ActionResult<IEnumerable<Designation>>> GetDesignations()
        {
            var designations = await _designationRepository.GetAllAsync();
            return Ok(designations);
        }

        // GET: api/Designation/5
        [HttpGet("GetDesignation/{id}")]
        public async Task<ActionResult<Designation>> GetDesignation(int id)
        {
            var designation = await _designationRepository.GetByIdAsync(id);
            if (designation == null)
                return NotFound();

            return Ok(designation);
        }

        // POST: api/Designation/InsertDesignation
        [HttpPost("InsertDesignation")]
        public async Task<ActionResult<Designation>> PostDesignation(Designation designation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate designation name
            bool exists = await _designationRepository.AnyAsync(d =>
                d.DesignationName.ToLower() == designation.DesignationName.ToLower());

            if (exists)
            {
                return Conflict($"Designation '{designation.DesignationName}' already exists.");
            }

            try
            {
                await _designationRepository.AddAsync(designation);
                return CreatedAtAction(nameof(GetDesignation),
                    new { id = designation.DesignationId }, designation);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Conflict($"Designation '{designation.DesignationName}' already exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the designation.");
            }
        }

        // PUT: api/Designation/UpdateDesignation/5
        [HttpPut("UpdateDesignation/{id}")]
        public async Task<IActionResult> UpdateDesignation(int id, Designation designation)
        {
            if (id != designation.DesignationId)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate name excluding current designation
            bool exists = await _designationRepository.AnyAsync(d =>
                d.DesignationId != id &&
                d.DesignationName.ToLower() == designation.DesignationName.ToLower());

            if (exists)
            {
                return Conflict($"Designation '{designation.DesignationName}' already exists.");
            }

            try
            {
                await _designationRepository.UpdateAsync(designation);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _designationRepository.ExistsAsync(id))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Conflict($"Designation '{designation.DesignationName}' already exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the designation.");
            }
        }


        // DELETE: api/Designation/5
        [HttpDelete("DeleteDesignation/{id}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            var designation = await _designationRepository.GetByIdAsync(id);
            if (designation == null)
                return NotFound();

            await _designationRepository.DeleteAsync(designation);
            return NoContent();
        }
    }
}