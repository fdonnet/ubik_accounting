using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.RolesAuthorizations.Services;
using Ubik.Security.Api.Mappers;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Events;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/rolesauthorizations")]
    public class RolesAuthorizationsAdminController(IRolesAuthorizationsCommandsService commandService, IRolesAuthorizationsQueriesService queryService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<RoleAuthorizationStandardResult>>> GetAll()
        {
            var results = (await queryService.GetAllAsync()).ToRoleAuthorizationStandardResults();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<RoleAuthorizationStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToRoleAuthorizationStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<RoleAuthorizationStandardResult>> AddAsync(AddRoleAuthorizationCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.ToRoleAuthorizationStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await commandService.ExecuteDeleteAsync(id);

            return result.Match<ActionResult>(
                Right: ok => NoContent(),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
