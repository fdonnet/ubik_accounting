using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Accounts.Errors;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Controller.v1
{
    /// <summary>
    /// For all queries endpoints => call the service manager and access the data
    /// For all commands endpoints => call the message bus
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController(IServiceManager serviceManager) : ControllerBase
    {
        private readonly IServiceManager _serviceManager = serviceManager;

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllAccountsResult>>> GetAll()
        {
            var result = (await _serviceManager.AccountService.GetAllAsync()).ToGetAllAccountResult();

            return Ok(result);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountResult>> Get(Guid id)
        {
            var result = await _serviceManager.AccountService.GetAsync(id);

            return result.Match(
                Right: r => Ok(r.ToGetAccountResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("AllAccountGroupLinks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountResult>> GetAllAccountsLinksToGroups()
        {
            var result = (await _serviceManager.AccountService.GetAllAccountGroupLinksAsync()).ToGetAllAccountGroupLinkResult();

            return Ok(result);
        }

        /// <summary>
        /// Account groups attached to this account (one account group per classification)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_account_read")]
        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("{id}/AccountGroups")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAccountGroupClassificationResult>>> GetAccountGroups(Guid id)
        {
            var result = await _serviceManager.AccountService.GetAccountGroupsWithClassificationInfoAsync(id);

            return result.Match(
                Right: r => Ok(r.ToGetAccountGroupClassificationResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountResult>> Add(AddAccountCommand command, IRequestClient<AddAccountCommand> client)
        {
            var (result,error) = await client.GetResponse<AddAccountResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var addedAccount = (await result).Message;
                return CreatedAtAction(nameof(Get), new { id = addedAccount.Id }, addedAccount);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateAccountResult>> Update(Guid id, 
            UpdateAccountCommand command, IRequestClient<UpdateAccountCommand> client)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("Account",id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var (result, error) = await client.GetResponse<UpdateAccountResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var updatedAccount = (await result).Message;
                return Ok(updatedAccount);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
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

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id, IRequestClient<DeleteAccountCommand> client)
        {
            var (result, error) = await client.GetResponse<DeleteAccountResult, 
                IServiceAndFeatureError>(new DeleteAccountCommand { Id = id});

            if (result.IsCompletedSuccessfully)
                return NoContent();
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
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
