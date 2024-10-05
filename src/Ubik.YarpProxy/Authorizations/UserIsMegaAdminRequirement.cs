using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ubik.YarpProxy.Services;

namespace Ubik.YarpProxy.Authorizations
{
    public class UserIsMegaAdminRequirement : IAuthorizationRequirement
    {

    }

    public class UserIsMegaAdminHandler(UserService userService) : AuthorizationHandler<UserIsMegaAdminRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsMegaAdminRequirement requirement)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var userInfo = await userService.GetUserInfoAsync(email);
                if (userInfo != null && userInfo.IsActivated && userInfo.IsMegaAdmin)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
            return;
        }
    }
}
