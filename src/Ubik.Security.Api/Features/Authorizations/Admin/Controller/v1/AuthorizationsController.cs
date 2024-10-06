using Asp.Versioning;
using LanguageExt;
using MassTransit;
using MassTransit.Futures.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.Authorizations.Services;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Results;

namespace Ubik.Security.Api.Features.Authorizations.Admin.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/[controller]")]
    public class AuthorizationsController(IAuthorizationsCommandsService commandService) : ControllerBase
    {
        //TODO: remove anonymous access
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AuthorizationStandardResult>> AddAsync(AddAuthorizationCommand command)
        {
            var result = await commandService.AddAsync(command);
            return result.Match(
               Right: ok => Ok(ok),
               Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));

            //TODO:Adpat when get
            //return result.Match(
            //    Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok),
            //    Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
