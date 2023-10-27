using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Commands.DeleteAccount;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;

namespace Ubik.Accounting.Api.Features.Accounts.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllAccountsResult>>> GetAll()
        {
            var results = await _mediator.Send(new GetAllAccountsQuery());
            return Ok(results);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountResult>> Get(Guid id)
        {
            var result = await _mediator.Send(new GetAccountQuery() { Id = id });
            return Ok(result);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountResult>> Add(AddAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateAccountResult>> Update(Guid id, UpdateAccountCommand command)
        {
            command.Id = id;
            var accountResult = await _mediator.Send(command);
            return Ok(accountResult);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteAccountCommand() { Id = id });

            return NoContent();
        }
    }
}
