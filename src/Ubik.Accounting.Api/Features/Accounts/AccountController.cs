using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;

namespace Ubik.Accounting.Api.Features.Accounts
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllAccountResult>>> GetAllAccounts()
        {
            var results = await _mediator.Send(new GetAllAccountQuery());
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountResult>> GetAccount(Guid id)
        {
            var result = await _mediator.Send(new GetAccountQuery() { Id = id });
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateAccountResult>> UpdateAccount(Guid id, UpdateAccountCommand updateAccountCommand)
        {
            updateAccountCommand.Id = id;
            var accountResult = await _mediator.Send(updateAccountCommand);
            return Ok(accountResult);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountResult>> AddAccount(AddAccountCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return CreatedAtAction(nameof(GetAccount), new {id =  result.Id}, result);
        }
    }
}
