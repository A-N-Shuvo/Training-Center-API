using Microsoft.AspNetCore.Authorization;
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
    public class DepartmentController : ControllerBase
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentController(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpGet("GetDepartments")]
        //[Authorize(Policy = "Department.View")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return Ok(departments);
        }

        //[Authorize(Policy = "Department.View")]
        // GET: api/Department/5
        [HttpGet("GetDepartment/{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            return Ok(department);
        }

        //[Authorize(Policy = "Department.Create")]
        [HttpPost("InsertDepartment")]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate department name
            bool deptExists = await _departmentRepository.AnyAsync(d =>
                d.DepartmentName.ToLower() == department.DepartmentName.ToLower());

            if (deptExists)
            {
                return Conflict($"A department with the name '{department.DepartmentName}' already exists.");
            }

            try
            {
                await _departmentRepository.AddAsync(department);
                return CreatedAtAction(nameof(GetDepartment),
                    new { id = department.DepartmentId }, department);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Conflict($"A department with the name '{department.DepartmentName}' already exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the department.");
            }
        }

        //[Authorize(Policy = "Department.Edit")]

        // PUT: api/Department/UpdateDepartment/5
        [HttpPut("UpdateDepartment/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate name excluding current department
            bool deptExists = await _departmentRepository.AnyAsync(d =>
                d.DepartmentId != id &&
                d.DepartmentName.ToLower() == department.DepartmentName.ToLower());

            if (deptExists)
            {
                return Conflict($"A department with the name '{department.DepartmentName}' already exists.");
            }

            try
            {
                await _departmentRepository.UpdateAsync(department);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _departmentRepository.ExistsAsync(id))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Conflict($"A department with the name '{department.DepartmentName}' already exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the department.");
            }
        }
        //[Authorize(Policy = "Department.Delete")]

        // DELETE: api/Department/5
        [HttpDelete("DeleteDepartment/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            await _departmentRepository.DeleteAsync(department);
            return NoContent();
        }
    }
}