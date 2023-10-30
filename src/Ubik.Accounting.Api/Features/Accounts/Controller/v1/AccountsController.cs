using Asp.Versioning;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;
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
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IServiceManager _serviceManager;

        public AccountsController(IMediator mediator, IServiceManager serviceManager)
        {
            _mediator = mediator;
            _serviceManager = serviceManager;
        }

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
            return result.IsSuccess
                ? (ActionResult<GetAccountResult>)Ok(result.Result.ToGetAccountResult())
                : (ActionResult<GetAccountResult>)new ObjectResult(result.Exception.ToValidationProblemDetails(HttpContext));
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountResult>> Add(AddAccountCommand command, IRequestClient<AddAccountCommand> client)
        {
            var (result,error) = await client.GetResponse<AddAccountResult, IServiceAndFeatureException>(command);

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
            command.Id = id;

            var (result, error) = await client.GetResponse<UpdateAccountResult, IServiceAndFeatureException>(command);

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

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id, IRequestClient<DeleteAccountCommand> client)
        {
            var (result, error) = await client.GetResponse<DeleteAccountResult, 
                IServiceAndFeatureException>(new DeleteAccountCommand { Id = id});

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
