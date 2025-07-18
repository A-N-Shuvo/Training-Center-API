using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TrainingCenter_Api.Models
{
    public class DynamicPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;
        const string PERMISSION_PREFIX = "Permissions.";

        public DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallback = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PERMISSION_PREFIX))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            return _fallback.GetPolicyAsync(policyName);
        }
    }
}