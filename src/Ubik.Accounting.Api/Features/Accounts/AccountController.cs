﻿using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Services;
using Ubik.ApiService.Common.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;

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

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountResult>> GetAccount(Guid id)
        {
            var result = await _mediator.Send(new GetAccountQuery() { Id = id });
            return Ok(result);
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
