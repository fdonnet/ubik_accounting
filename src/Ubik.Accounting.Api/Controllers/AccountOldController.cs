using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Dto.Mappers;
using Ubik.Accounting.Api.Services;
using Ubik.ApiService.Common.Controllers;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountOldController : ControllerBase
    {
        private readonly IChartOfAccountsService _chartOfAccountsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountOldController(IChartOfAccountsService chartOfAccountsService, IHttpContextAccessor httpContextAccessor)
        {
            _chartOfAccountsService = chartOfAccountsService;
            _httpContextAccessor = httpContextAccessor; 
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult> AUpdatedd(Guid id, AccountDto account)
        {
            var accountResult = await _chartOfAccountsService.UpdateAccountAsync(id, account);
            return accountResult.ToNoContent(_httpContextAccessor);
        }
    }
}
