using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Users.Results;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Api.Features.Users.Mappers;

namespace Ubik.Security.Api.Features.Users.Controller.v1
{
    /// <summary>
    /// For all queries endpoints => call the service manager and access the data
    /// For all commands endpoints => call the message bus
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IServiceManager serviceManager) : ControllerBase
    {
        //[Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetUserResult>> Get(Guid id)
        {
            var result = await serviceManager.UserManagementService.GetAsync(id);
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
        public async Task<ActionResult<AddUserResult>> Register(AddUserCommand command, IRequestClient<AddUserCommand> client)
        {
            var (result, error) = await client.GetResponse<AddUserResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var added = (await result).Message;

                return CreatedAtAction(nameof(Get), new { id = added.Id }, added);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }
    }
}
