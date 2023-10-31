using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Asp.Versioning;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using MassTransit;

namespace Ubik.Accounting.Api.Features.AccountGroups.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountGroupsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AccountGroupsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get all account groups
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllAccountGroupsResult>>> GetAll()
        {
            var results = (await _serviceManager.AccountGroupService.GetAllAsync()).ToGetAllAccountGroupsResult();
            return Ok(results);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountGroupResult>> Get(Guid id)
        {
            var result = await _serviceManager.AccountGroupService.GetAsync(id);
            return result.IsSuccess
                ? (ActionResult<GetAccountGroupResult>)Ok(result.Result.ToGetAccountGroupResult())
                : (ActionResult<GetAccountGroupResult>)new ObjectResult(result.Exception.ToValidationProblemDetails(HttpContext));
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("{id}/Accounts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountGroupResult>> GetChildAccount(Guid id)
        {
            var result = await _serviceManager.AccountGroupService.GetWithChildAccountsAsync(id);

            return result.IsSuccess
                ? (ActionResult<GetAccountGroupResult>)Ok(result.Result.Accounts!.ToGetChildAccountsResult())
                : (ActionResult<GetAccountGroupResult>)new ObjectResult(result.Exception.ToValidationProblemDetails(HttpContext));
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountGroupResult>> Add(AddAccountGroupCommand command, IRequestClient<AddAccountGroupCommand> client)
        {
            var (result, error) = await client.GetResponse<AddAccountGroupResult, IServiceAndFeatureException>(command);

            if (result.IsCompletedSuccessfully)
            {
                var addedAccountGroup = (await result).Message;
                return CreatedAtAction(nameof(Get), new { id = addedAccountGroup.Id }, addedAccountGroup);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateAccountGroupResult>> Update(Guid id,
            UpdateAccountGroupCommand command, IRequestClient<UpdateAccountGroupCommand> client)
        {
            command.Id = id;

            var (result, error) = await client.GetResponse<UpdateAccountGroupResult, IServiceAndFeatureException>(command);

            if (result.IsCompletedSuccessfully)
            {
                var updatedAccountGroup = (await result).Message;
                return Ok(updatedAccountGroup);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id, IRequestClient<DeleteAccountGroupCommand> client)
        {
            var (result, error) = await client.GetResponse<DeleteAccountGroupResult,
            IServiceAndFeatureException>(new DeleteAccountGroupCommand { Id = id });

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
