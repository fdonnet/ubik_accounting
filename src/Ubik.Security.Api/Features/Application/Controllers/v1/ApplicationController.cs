﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Features.Application.Services;

namespace Ubik.Security.Api.Features.Application.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("admin/api/v{version:apiVersion}/application")]
    public class ApplicationController(IApplicationCommandService commandService) : ControllerBase
    {
        [HttpDelete("cleanupdb")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> CleanupDatabaseInDev()
        {
            var result = await commandService.CleanupDatabaseInDevAsync();
            return result ? Ok(result) : StatusCode(500);
        }
    }
}
