using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Structure.Api.Features.Accounts.Services;
using Ubik.Accounting.Structure.Api.Mappers;
using Ubik.Accounting.Structure.Contracts.Accounts.Commands;
using Ubik.Accounting.Structure.Contracts.Accounts.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController(IAccountQueryService queryService, IAccountCommandService commandService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountStandardResult>>> GetAll()
        {
            var result = (await queryService.GetAllAsync()).ToAccountStandardResults();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AccountStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);

            return result.Match(
                Right: r => Ok(r.ToAccountStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpGet("accountgrouplinks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountGroupLinkResult>>> GetAllAccountsLinksToGroups()
        {
            var result = (await queryService.GetAllAccountGroupLinksAsync()).ToAccountGroupLinkResults();

            return Ok(result);
        }

        [HttpGet("{id}/accountgroups")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountGroupWithClassificationResult>>> GetAccountGroups(Guid id)
        {
            var result = await queryService.GetAccountGroupsWithClassificationInfoAsync(id);

            return result.Match(
                Right: r => Ok(r.ToAccountGroupWithClassificationResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AccountStandardResult>> Add(AddAccountCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                Right: r => CreatedAtAction(nameof(Get), new { id = r.Id }, r.ToAccountStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AccountStandardResult>> Update(Guid id, UpdateAccountCommand command)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchWithCommandError("Account",id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.UpdateAsync(command);

            return result.Match(
                Right: r => Ok(r.ToAccountStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost("{id}/accountgroups/{accountGroupId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AccountInAccountGroupResult>> AddInAccountGroup(Guid id, Guid accountGroupId)
        {
            var result = await commandService.AddInAccountGroupAsync(new AddAccountInAccountGroupCommand
            {
                AccountId = id,
                AccountGroupId = accountGroupId
            });

            return result.Match(
                Right: r => Ok(r.ToAccountInAccountGroupStandardResult()),
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
                Right: r => NoContent(),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpDelete("{id}/accountGroups/{accountGroupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> DeleteFromAccountGroup(Guid id, Guid accountGroupId)
        {
            var result = await commandService.DeleteFromAccountGroupAsync(new DeleteAccountInAccountGroupCommand
            {
                AccountId = id,
                AccountGroupId = accountGroupId
            });

            return result.Match<ActionResult>(
                Right: r => NoContent(),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
