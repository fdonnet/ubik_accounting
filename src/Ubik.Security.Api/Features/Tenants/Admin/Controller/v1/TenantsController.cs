using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.Tenants.Admin.Services;
using Ubik.Security.Api.Features.Tenants.Mappers;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Events;

namespace Ubik.Security.Api.Features.Tenants.Admin.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/[controller]")]
    public class TenantsController(ITenantsAdminCommandsService commandService, ITenantsAdminQueriesService queryService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<TenantStandardResult>>> GetAll()
        {
            var results = (await queryService.GetAllAsync()).ToTenantStandardResults();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TenantStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToTenantStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TenantStandardResult>> AddAsync(AddTenantCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.ToTenantStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TenantStandardResult>> Update(Guid id, UpdateTenantCommand command)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("Tenant", id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.UpdateAsync(command);

            return result.Match(
                Right: ok => Ok(ok.ToTenantStandardResult()),
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
