using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.Roles.Admin.Services;
using Ubik.Security.Api.Features.Roles.Mappers;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Results;
using Ubik.Security.Contracts.Roles.Commands;
using Ubik.Security.Contracts.Roles.Results;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Roles.Admin.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/[controller]")]
    public class RolesController(IRolesAdminCommandsService commandService, IRolesAdminQueriesService queryService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<RoleStandardResult>>> GetAll()
        {
            var results = (await queryService.GetAllAsync()).ToRoleStandardResults();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<RoleStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToRoleStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<RoleStandardResult>> AddAsync(AddRoleCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.ToRoleStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<RoleStandardResult>> Update(Guid id, UpdateRoleCommand command)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("Role", id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.UpdateAsync(command);

            return result.Match(
                Right: ok => Ok(ok.ToRoleStandardResult()),
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
