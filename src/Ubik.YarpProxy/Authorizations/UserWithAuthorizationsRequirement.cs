using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ubik.YarpProxy.Services;

namespace Ubik.YarpProxy.Authorizations
{
    public class UserWithAuthorizationsRequirement(List<string> authorizationsNeeded) : IAuthorizationRequirement
    {
        public List<string> AuthorizationsNeeded { get; set; } = authorizationsNeeded;
    }

    public class UserWithAuthorizationsHandler(UserService userService) : AuthorizationHandler<UserWithAuthorizationsRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserWithAuthorizationsRequirement requirement)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var userInfo = await userService.GetUserInfoAsync(email);

                if (userInfo != null
                    && userInfo.SelectedTenantId != null
                    && userInfo.AuthorizationsByTenantIds.TryGetValue((Guid)userInfo.SelectedTenantId, out var auth))
                {
                    //TODO: Can be optimized except here is not very good or maybe it is
                    if (auth != null && !requirement.AuthorizationsNeeded.Except(auth.Select(a=>a.Code)).Any())
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
            context.Fail();
            return;
        }
    }
}
