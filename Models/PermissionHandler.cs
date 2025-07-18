using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TrainingCenter_Api.Data;

namespace TrainingCenter_Api.Models
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var username = context.User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return;

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = _context.RolePermissions
                .Where(rp => roles.Contains(rp.RoleName))
                .Select(rp => rp.Permission)
                .ToList();

            if (permissions.Contains(requirement.Permission))
                context.Succeed(requirement);
        }
    }
}
