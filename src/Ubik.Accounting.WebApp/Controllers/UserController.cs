using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ubik.Accounting.WebApp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("/account/login")]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/"
            });
        }

        [HttpPost("/account/logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
