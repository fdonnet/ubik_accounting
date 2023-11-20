using Microsoft.AspNetCore.Authentication;

namespace Ubik.Accounting.WebApp.Security
{
    public class UserServiceMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task InvokeAsync(HttpContext context, UserService service)
        {
            service.SetUser(context.User);
            var accessToken = await context.GetTokenAsync("access_token");
            var refreshToken = await context.GetTokenAsync("refresh_token");
            service.SetToken(new TokenProvider { AccessToken = accessToken, RefreshToken = refreshToken });
            await next(context);
        }
    }
}
