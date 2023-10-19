﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;

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

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountResult>> Add(AddAccountCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

    }
}
