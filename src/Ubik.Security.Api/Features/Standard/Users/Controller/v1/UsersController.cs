using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Users.Results;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Api.Features.Standard.Users.Services;
using Ubik.Security.Api.Features.Standard.Users.Mappers;

namespace Ubik.Security.Api.Features.Standard.Users.Controller.v1
{
    /// <summary>
    /// For all queries endpoints => call the service manager and access the data
    /// For all commands endpoints => call the message bus
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IUsersCommandsService commandService, IUsersQueriesService queryService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetUserResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToGetUserResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        //TODO: need to be protected by captach or other stuff
        //This API need to remain private (no public call on that)
        //Maybe protect by domain names or other stuff but it allow a user to register
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddUserResult>> Register(AddUserCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok),
                Left: ko => new ObjectResult(ko.ToValidationProblemDetails(HttpContext)));
        }
    }
}
