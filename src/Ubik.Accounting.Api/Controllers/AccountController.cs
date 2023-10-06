﻿using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Dto.Mappers;
using Ubik.Accounting.Api.Services;

namespace Ubik.Accounting.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IChartOfAccountsService _chartOfAccountsService;

        public AccountController(IChartOfAccountsService chartOfAccountsService)
        {
            _chartOfAccountsService = chartOfAccountsService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<AccountDto>>> Get() 
        { 
            var accounts = await _chartOfAccountsService.GetAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("withAccountGroup")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<AccountWithAccountGroupDto>>> GetWithAccountGroup()
        {
            var accounts = await _chartOfAccountsService.GetAccountsWithAccountGroupAsync();
            return Ok(accounts);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<ActionResult<AccountDto>> Add(AccountDtoForAdd accountToAdd)
        {
            var accountResult = await _chartOfAccountsService.AddAccountAsync(accountToAdd);
            return CreatedAtAction(nameof(Get), new { id = accountResult.Id }, AccountMapper.ToAccountDto(accountResult));
        }
    }
}
