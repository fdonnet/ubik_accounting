using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ubik.Accounting.WebApp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("/Login")]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/"
            });
        }

        [HttpPost("/Logout")]
        public async Task Logout(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
