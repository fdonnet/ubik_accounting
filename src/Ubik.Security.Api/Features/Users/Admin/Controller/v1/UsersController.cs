using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Admin.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/users")]
    public class UsersController(IUsersQueriesService queryService) : ControllerBase
    {
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UserAdminResult>> Get(string email)
        {
            var result = await queryService.GetAsync(email);
            return result.Match(
                            Right: ok => Ok(ok.ToUserAdminResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
