using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleController> _logger;
        private readonly ApplicationDbContext _context;


        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RoleController> logger, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }


        // নতুন Role তৈরি করার এন্ডপয়েন্ট
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (string.IsNullOrEmpty(model.RoleName))
                return BadRequest("Role name is required");

            var roleExist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (roleExist)
                return BadRequest("Role already exists");

            var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = $"Role {model.RoleName} created successfully" });
        }

        // User-কে Role অ্যাসাইন করার এন্ডপয়েন্ট
        [HttpPost("assign-role-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return NotFound("User not found");

            var roleExist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExist)
                return NotFound("Role not found");

            var isUserInRole = await _userManager.IsInRoleAsync(user, model.RoleName);
            if (isUserInRole)
                return BadRequest("User already has this role");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);


            return Ok(new { message = $"Role {model.RoleName} assigned to user {model.Username} successfully" });


        }

        // User থেকে Role রিমুভ করার এন্ডপয়েন্ট
        [HttpPost("remove-role-from-user")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return NotFound("User not found");

            var isUserInRole = await _userManager.IsInRoleAsync(user, model.RoleName);
            if (!isUserInRole)
                return BadRequest("User does not have this role");

            var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);


            return Ok(new { message = $"Role {model.RoleName} removed from user {model.Username} successfully" });


        }


        // Get all roles
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok( new { roles});
        }

        // Get all users (simplified version)
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users.Select(u => new { u.Id, u.FullName, u.UserName, u.PasswordHash, u.Email, u.PhoneNumber, u.IsActive }).ToList();
            return Ok( new { users});
        }


        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole([FromQuery] string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = $"Role {roleName} deleted successfully" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(model.OldRoleName))
                    return BadRequest("Old role name is required");

                if (string.IsNullOrWhiteSpace(model.NewRoleName))
                    return BadRequest("New role name is required");

                // Check if the old role exists
                var existingRole = await _roleManager.FindByNameAsync(model.OldRoleName);
                if (existingRole == null)
                    return NotFound($"Role '{model.OldRoleName}' not found");

                // Check if the new role name already exists
                var roleExists = await _roleManager.RoleExistsAsync(model.NewRoleName);
                if (roleExists)
                    return Conflict($"Role '{model.NewRoleName}' already exists");

                // Update the role
                existingRole.Name = model.NewRoleName;
                var result = await _roleManager.UpdateAsync(existingRole);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update role: {Errors}", result.Errors);
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        $"Failed to update role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                _logger.LogInformation("Role '{OldRoleName}' updated to '{NewRoleName}'",
                    model.OldRoleName, model.NewRoleName);

                return Ok(new
                {
                    Message = $"Role '{model.OldRoleName}' successfully updated to '{model.NewRoleName}'"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role from '{OldRoleName}' to '{NewRoleName}'",
                    model.OldRoleName, model.NewRoleName);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    "An error occurred while updating the role");
            }
        }
        [HttpGet("GetRolePermissions")]
        public IActionResult GetRolePermissions(string roleName)
        {
            var permissions = _context.RolePermissions
                .Where(rp => rp.RoleName == roleName)
                .Select(rp => rp.Permission)
                .ToList();

            return Ok(permissions);
        }

        [HttpPost("UpdateRolePermissions")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionsModel model)
        {
            // First remove existing permissions for this role
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleName == model.RoleName)
            .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            // Add new permissions
            foreach (var permission in model.Permissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleName = model.RoleName,
                    Permission = permission
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        // RoleController.cs এ নিচের মেথড যোগ করুন
        [HttpGet("GetAllModules")]
        public IActionResult GetAllModules()
        {
            var modules = new List<string>
            {
                "Department",

                // Others modules
            };
            return Ok(modules);
        }

        [HttpGet("GetModulePermissions")]
        public IActionResult GetModulePermissions(string module)
        {
            var permissions = new List<string>();

            switch (module)
            {
                case "Department":
                    permissions = new List<string> { "Create", "Edit", "Delete", "View" };
                    break;
               
                // Other modules
                default:
                    permissions = new List<string> { "View" };
                    break;
            }

            return Ok(permissions);
        }

        [HttpGet("GetUserPermissions")]
        [Authorize]
        public async Task<IActionResult> GetUserPermissions()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = _context.RolePermissions
                .Where(rp => roles.Contains(rp.RoleName))
                .Select(rp => rp.Permission)
                .ToList();

            return Ok(permissions);
        }

        [HttpDelete("deactivate/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    return NotFound("User not found");

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(new { message = $"User {username} deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("reactivate/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateUser(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    return NotFound("User not found");

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(new { message = $"User {username} reactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
    public class UpdateRolePermissionsModel
    {
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; }
    }
    public class UpdateRoleModel
    {
        public string OldRoleName { get; set; }
        public string NewRoleName { get; set; }
    }
    public class RoleModel
    {
        public string RoleName { get; set; }
    }

    public class AssignRoleModel
    {
        public string Username { get; set; }
        public string RoleName { get; set; }
    }
}