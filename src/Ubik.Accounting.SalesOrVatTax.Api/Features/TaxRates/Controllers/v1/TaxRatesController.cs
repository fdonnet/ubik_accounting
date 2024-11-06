using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.SalesOrVatTax.Api.Features.TaxRates.Services;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.TaxRates.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/sales-vat-tax/[controller]")]
    public class TaxRatesController(ITaxRateCommandService commandService, ITaxRateQueryService queryService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<SalesOrVatTaxRateStandardResult>>> GetAll()
        {
            var results = (await queryService.GetAllAsync()).ToSalesOrVatTaxRateStandardResults();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<SalesOrVatTaxRateStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToSalesOrVatTaxRateStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<SalesOrVatTaxRateStandardResult>> AddAsync(TaxRateCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.ToSalesOrVatTaxRateStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<SalesOrVatTaxRateStandardResult>> Update(Guid id, UpdateSalesOrVatTaxRateCommand command)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchWithCommandError("TaxRate", id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.UpdateAsync(command);

            return result.Match(
                Right: ok => Ok(ok.ToSalesOrVatTaxRateStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await commandService.DeleteAsync(id);

            return result.Match<ActionResult>(
                Right: ok => NoContent(),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
