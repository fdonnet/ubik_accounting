using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Structure.Api.Features.Application.Services;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Structure.Api.Features.Application.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/struct-app")]
    public class ApplicationController(IApplicationCommandService commandService) : ControllerBase
    {
        //Cannot be used in PROD, command and queries DB can be different
        [HttpDelete("cleanupdb")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> CleanupDatabaseInDev()
        {
            var result = await commandService.CleanupDatabaseInDevAsync();
            return result ? Ok(result) : StatusCode(500);
        }

        [HttpGet("health")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> IsReady()
        {
            var result = await commandService.IsReady();
            return Ok(result);
        }
    }
}
