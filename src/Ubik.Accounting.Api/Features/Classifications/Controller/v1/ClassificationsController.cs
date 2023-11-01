﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ClassificationsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ClassificationsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllClassificationsResult>>> GetAll()
        {
            var results = (await _serviceManager.ClassificationService.GetAllAsync()).ToGetAllClassificationsResult();
            return Ok(results);
        }

    }
}