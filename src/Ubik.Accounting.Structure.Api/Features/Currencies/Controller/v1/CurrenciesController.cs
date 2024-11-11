using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Structure.Api.Features.Currencies.Services;
using Ubik.Accounting.Structure.Api.Mappers;
using Ubik.Accounting.Structure.Contracts.Currencies.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Structure.Api.Features.Currencies.Controller.v1
{

    //TODO: to be replaced with the real source of truth API (currencies will not be managed in accounting structure)
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CurrenciesController (ICurrencyQueryService queryService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<CurrencyStandardResult>>> GetAll()
        {
            var results = (await queryService.GetAllAsync()).ToCurrencyStandardResults();
            return Ok(results);
        }
    }
}
