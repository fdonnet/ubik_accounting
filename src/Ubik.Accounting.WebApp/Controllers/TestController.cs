using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ubik.Accounting.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("/Hello")]
        public ActionResult Hello()
        {
            return Ok("Hi!");
        }
    }
}
