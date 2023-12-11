using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.Contracts.Accounts.Commands;


namespace Ubik.Accounting.WebApp.Controllers
{
    /// <summary>
    /// Works as a reverse proxy for interactive automode (wasm component)
    /// </summary>
    /// <param name="client"></param>
    [Authorize]
    [ApiController]
    public class ReverseProxyWasmController(IAccountingApiClient client) : ControllerBase
    {
        readonly IAccountingApiClient client = client;

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("/GetAllAccounts")]
        public async Task AccountsList()
        {
            var response = await client.GetAllAccountsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPost("/AddAccount")]
        public async Task AddAccount(AddAccountCommand command)
        {
            var response = await client.AddAccountsAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("/GetAccount/{id}")]
        public async Task Account(Guid id)
        {
            var response = await client.GetAccountAsync(id);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_currency_read")]
        [HttpGet("/GetAllCurrencies")]
        public async Task CurrenciesList()
        {
            var response = await client.GetAllCurrenciesAsync();
            await ForwardResponse(response);
        }

        private async Task ForwardResponse(HttpResponseMessage? responseMsg)
        {
            if (responseMsg == null) return;

            HttpContext.Response.StatusCode = (int)responseMsg.StatusCode;
            await responseMsg.Content.CopyToAsync(HttpContext.Response.Body);
        }
    }
}
