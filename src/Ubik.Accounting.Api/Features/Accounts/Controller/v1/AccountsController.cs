using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.Mappers;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController(IAccountQueryService queryService, IAccountCommandService commandService, IServiceManager serviceManager) : ControllerBase
    {
        private readonly IServiceManager _serviceManager = serviceManager;

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

        /// <summary>
        /// Account groups attached to this account (one account group per classification)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            var result = await commandService.AddAsync(command.ToAccount());

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
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("Account",id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.UpdateAsync(command.ToAccount());

            return result.Match(
                Right: r => Ok(r.ToAccountStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Attach an account group to the account (one per classification)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountGroupId"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_account_write")]
        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost("{id}/AccountGroups/{accountGroupId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountInAccountGroupResult>> AddInAccountGroup(Guid id,
            Guid accountGroupId, IRequestClient<AddAccountInAccountGroupCommand> client)
        {
            var (result, error) = await client.GetResponse<AddAccountInAccountGroupResult
                , IServiceAndFeatureError>(new AddAccountInAccountGroupCommand { AccountId = id, AccountGroupId=accountGroupId});

            if (result.IsCompletedSuccessfully)
            {
                var accountAccountGroup = (await result).Message;
                return Ok(accountAccountGroup);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
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

        /// <summary>
        /// Remove an account group from the account (detach)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountGroupId"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_account_write")]
        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpDelete("{id}/AccountGroups/{accountGroupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> DeleteFromAccountGroup(Guid id, Guid accountGroupId, IRequestClient<DeleteAccountInAccountGroupCommand> client)
        {
            var (result, error) = await client.GetResponse<DeleteAccountInAccountGroupResult,
                IServiceAndFeatureError>(new DeleteAccountInAccountGroupCommand { AccountId = id,  AccountGroupId=accountGroupId });

            if (result.IsCompletedSuccessfully)
                return NoContent();
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }
    }
}
