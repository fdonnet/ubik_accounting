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
            //var expiryAt = await context.GetTokenAsync("expires_at");

            var expiryAt = await context.GetTokenAsync("expires_at");

            if (!DateTimeOffset.TryParse(expiryAt, out DateTimeOffset refreshAt)
                || refreshAt < DateTimeOffset.UtcNow
                || string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshAt = DateTimeOffset.MaxValue;
            }
            else
            {
                var seconds = refreshAt.Subtract(DateTimeOffset.UtcNow).TotalSeconds;
                refreshAt = DateTimeOffset.UtcNow.AddSeconds(seconds / 2);
            }


            service.SetToken(new TokenProvider { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAt = expiryAt, RefreshAt=refreshAt });
            await next(context);
        }
    }
}
