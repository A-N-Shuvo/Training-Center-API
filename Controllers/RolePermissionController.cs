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

    public class RolePermissionController : ControllerBase
    {
        private readonly IRepository<RolePermission> _repository;

        public RolePermissionController(IRepository<RolePermission> repository)
        {
            _repository = repository;
            
        }

        // GET: api/RolePermissions
        [HttpGet("GetRolePermissions")]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetRolePermissions()
        {
            var rolePermissions = await _repository.GetAllAsync();
            return Ok(rolePermissions);
        }

        // GET: api/RolePermissions/5
        [HttpGet("GetRolePermission/{id}")]
        public async Task<ActionResult<RolePermission>> GetRolePermission(int id)
        {
            var rolePermission = await _repository.GetByIdAsync(id);

            if (rolePermission == null)
            {
                return NotFound();
            }

            return Ok(rolePermission);
        }

        
        [HttpPut, Route("UpdateRolePermission/{id}")]
        public async Task<IActionResult> UpdateRolePermission(int id, RolePermission rolePermission)
        {
            if (id != rolePermission.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.UpdateAsync(rolePermission);
            }
            catch
            {
                if (!await _repository.ExistsAsync(id))
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


        // POST: api/RolePermissions
        [HttpPost("InsertRolePermission")]
        public async Task<IActionResult> InsertRolePermission([FromBody] RolePermissionInsertModel model)
        {
            if (string.IsNullOrEmpty(model.RoleName) || model.Permissions == null || !model.Permissions.Any())
                return BadRequest("RoleName and Permissions are required");

            var rolePermissions = model.Permissions.Select(permission => new RolePermission
            {
                RoleName = model.RoleName,
                Permission = permission
            }).ToList();

            foreach (var rp in rolePermissions)
            {
                await _repository.AddAsync(rp);
            }

            return Ok(new { message = "Permissions added successfully", count = rolePermissions.Count });
        }


        [HttpDelete("DeleteRolePermission/{id}")]
        public async Task<IActionResult> DeleteRolePermission(int id)
        {
            var day = await _repository.GetByIdAsync(id);
            if (day == null)
                return NotFound();

            await _repository.DeleteAsync(day);
            return NoContent();
        }


    }
    public class RolePermissionInsertModel
    {
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; }
    }
}