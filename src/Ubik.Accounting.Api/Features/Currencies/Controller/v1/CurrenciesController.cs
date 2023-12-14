using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Currencies.Mappers;
using Ubik.Accounting.Contracts.Currencies.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Currencies.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CurrenciesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "ubik_accounting_currency_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllCurrenciesResult>>> GetAll()
        {
            var results = (await _serviceManager.CurrencyService.GetAllAsync()).ToGetAllCurrenciesResult();
            return Ok(results);
        }
    }
}
