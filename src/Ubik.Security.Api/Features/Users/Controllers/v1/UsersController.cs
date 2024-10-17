using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Users.Results;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Api.Features.Mappers;

namespace Ubik.Security.Api.Features.Users.Controllers.v1
{
    /// <summary>
    /// For all queries endpoints => call the service manager and access the data
    /// For all commands endpoints => call the message bus
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IUsersCommandsService commandService, IUsersQueriesService queryService) : ControllerBase
    {
        //Get the user but only for the current selected tenant
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UserStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetUserInSelectedTenantAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToUserStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        //TODO: need to be protected by captach or other stuff
        //This API need to remain private (no public call on that)
        //Maybe protect by domain names or other stuff but it allow a user to register
        //See if we chain the other needed stuff here or in a second step
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UserStandardResult>> Register(AddUserCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.ToUserStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpGet("{id}/roles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UserStandardResult>> GetAllUserRolesInTenant(Guid id)
        {
            var result = await queryService.GetUserRolesInSelectedTenantAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToRoleStandardResults()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }


    }
}
