﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Features.Tenants.Mappers;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Results;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MeController(IUsersQueriesService queryService, IUsersCommandsService commandService, ICurrentUser currentUser) : ControllerBase
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

        [HttpGet("tenants/selected")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TenantStandardResult>> GetMySelectedTenant()
        {
            var result = await queryService.GetUserSelectedTenantAsync(currentUser.Id);
            return result.Match(
                            Right: ok => Ok(ok.ToTenantStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpGet("tenants/{tenantId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TenantStandardResult>> GetTenant(Guid tenantId)
        {
            //TODO
            var result = await queryService.GetUserSelectedTenantAsync(currentUser.Id);
            return result.Match(
                            Right: ok => Ok(ok.ToTenantStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost("tenants")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TenantForUserResult>> AddTenant(AddTenantCommand command)
        {
            var result = await commandService.AddNewTenantAndAttachToTheUser(currentUser.Id,command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(GetTenant), new { tenantId = ok.Id }, ok.ToUserForTenantResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
