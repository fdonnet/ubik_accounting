﻿using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Results;

namespace Ubik.Security.Api.Features.Authorizations.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthorizationsController : ControllerBase
    {
        //TODO: remove anonymous access
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAuthorizationResult>> AddAsync(AddAuthorizationCommand command, IRequestClient<AddAuthorizationCommand> client)
        {
            var (result, error) = await client.GetResponse<AddAuthorizationResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var added = (await result).Message;
                //TODO: change created action
                //return CreatedAtAction(nameof(Get), new { id = added.Id }, added);
                return Ok(added);

            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }
    }
}
