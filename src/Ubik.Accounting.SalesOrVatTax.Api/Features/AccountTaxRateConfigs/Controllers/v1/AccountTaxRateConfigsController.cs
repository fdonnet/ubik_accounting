using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Services;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountLinkedTaxRates.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/accounts")]
    public class AccountTaxRateConfigsController(IAccountTaxRateConfigsQueryService queryService,
        IAccountTaxRateConfigsCommandService commandService) : ControllerBase
    {
        [HttpGet("{id}/taxrates")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountTaxRateConfigStandardResult>>> GetAll(Guid id)
        {
            var results = await queryService.GetAllAsync(id);

            return results.Match(
                Right: ok => Ok(ok.ToAccountTaxRateConfigStandardResults()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost("{id}/taxrates/{taxRateId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AccountTaxRateConfigStandardResult>> AddAccountTaxRateConfig
            (Guid id, Guid taxRateId, AddAccountTaxRateConfigCommand command)
        {
            if (command.AccountId != id)
                return new ObjectResult(new ResourceIdNotMatchWithCommandError("Account", id, command.AccountId)
                    .ToValidationProblemDetails(HttpContext));

            if (command.TaxRateId != taxRateId)
                return new ObjectResult(new ResourceIdNotMatchWithCommandError("TaxRate", id, command.TaxRateId)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.AttachAsync(command); 

            return result.Match(
                Right: r => Ok(r.ToAccountTaxRateConfigStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpDelete("{id}/taxrates/{taxRateId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AccountTaxRateConfigStandardResult>> DeleteAccountTaxRateConfig(Guid id, Guid taxRateId)
        {
            var result = await commandService.DetachAsync(new DeleteAccountTaxRateConfigCommand()
            {
                AccountId = id,
                TaxRateId = taxRateId
            });

            return result.Match<ActionResult>(
                Right: r => NoContent(),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
