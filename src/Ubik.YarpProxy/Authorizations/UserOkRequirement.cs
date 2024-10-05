using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Claims;
using Ubik.YarpProxy.Services;

namespace Ubik.YarpProxy.Authorizations
{
    public class UserInfoOkRequirement : IAuthorizationRequirement
    {

    }
    public class UserInfoOkHandler(UserService userService) : AuthorizationHandler<UserInfoOkRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserInfoOkRequirement requirement)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var userInfo = await userService.GetUserInfoAsync(email);
                if (userInfo != null && userInfo.IsActivated)
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
