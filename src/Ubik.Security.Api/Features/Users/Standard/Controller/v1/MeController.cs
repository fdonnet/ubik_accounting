using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Standard.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MeController(IUsersQueriesService queryService, ICurrentUser currentUser) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UserStandardResult>> Get()
        {
            var result = await queryService.GetAsync(currentUser.Id);
            return result.Match(
                            Right: ok => Ok(ok.ToUserStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
