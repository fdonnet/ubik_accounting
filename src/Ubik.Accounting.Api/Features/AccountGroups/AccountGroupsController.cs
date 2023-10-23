using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.AddAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.DeleteAccountGroup;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetChildAccounts;
using System.Security.Claims;

namespace Ubik.Accounting.Api.Features.AccountGroups
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountGroupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountGroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllAccountGroupsResult>>> GetAll()
        {
            var results = await _mediator.Send(new GetAllAccountGroupsQuery());
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
            var result = await _mediator.Send(new GetAccountGroupQuery() { Id = id });
            return Ok(result);
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
            var result = await _mediator.Send(new GetChildAccountsQuery() { AccountGroupId = id });
            return Ok(result);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountGroupResult>> Add(AddAccountGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateAccountGroupResult>> Update(Guid id, UpdateAccountGroupCommand command)
        {
            command.Id = id;
            var accountResult = await _mediator.Send(command);
            return Ok(accountResult);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteAccountGroupCommand() { Id = id });

            return NoContent();
        }

    }
}
