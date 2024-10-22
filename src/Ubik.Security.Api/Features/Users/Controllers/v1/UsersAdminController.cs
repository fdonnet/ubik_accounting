using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/users")]
    public class UsersAdminController(IUsersQueriesService queryService) : ControllerBase
    {
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UserAdminOrMeResult>> Get(string email)
        {
            var result = await queryService.GetUserWithAuhtorizationsByTenants(email);

            return result.Match(
                            Right: ok => Ok(ok),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
