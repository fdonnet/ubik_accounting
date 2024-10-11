using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ubik.YarpProxy.Services;

namespace Ubik.YarpProxy.Authorizations
{
    public class UserInfoOnlyRequirement(bool needsMegaAdminRight) : IAuthorizationRequirement
    {
        public bool NeedsMegaAdminRight { get; set; } = needsMegaAdminRight;
    }
    public class UserInfoOkHandler(UserService userService) : AuthorizationHandler<UserInfoOnlyRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserInfoOnlyRequirement requirement)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var userInfo = await userService.GetUserInfoAsync(email);

                if (requirement.NeedsMegaAdminRight)
                {
                    if (userInfo != null && userInfo.IsActivated && userInfo.IsMegaAdmin)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
                else
                {
                    if (userInfo != null && userInfo.IsActivated)
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
