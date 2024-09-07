using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;


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
        [HttpGet("/Accounts")]
        public async Task AccountsList()
        {
            var response = await client.GetAllAccountsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("/Accounts/{id}")]
        public async Task Account(Guid id)
        {
            var response = await client.GetAccountAsync(id);
            await ForwardResponse(response);
        }


        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPost("/Accounts")]
        public async Task AddAccount(AddAccountCommand command)
        {
            var response = await client.AddAccountAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPut("/Accounts/{id}")]
        public async Task UpdateAccount(Guid id, UpdateAccountCommand command)
        {
            var response = await client.UpdateAccountAsync(id,command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpDelete("/Accounts/{id}")]
        public async Task DeleteAccount(Guid id)
        {
            var response = await client.DeleteAccountAsync(id);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_currency_read")]
        [HttpGet("/Currencies")]
        public async Task CurrenciesList()
        {
            var response = await client.GetAllCurrenciesAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet("/Classifications")]
        public async Task ClassificationsList()
        {
            var response = await client.GetAllClassificationsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpPost("/Classifications")]
        public async Task AddClassification(AddClassificationCommand command)
        {
            var response = await client.AddClassificationAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpPut("/Classifications/{id}")]
        public async Task UpdateClassification(Guid id, UpdateClassificationCommand command)
        {
            var response = await client.UpdateClassificationAsync(id, command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("/AccountGroups")]
        public async Task AccountGroupsList()
        {
            var response = await client.GetAllAccountGroupsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("/Accounts/AllAccountGroupLinks")]
        public async Task AccountGroupLinks()
        {
            var response = await client.GetAllAccountsLinksAsync();
            await ForwardResponse(response);
        }

        private async Task ForwardResponse(HttpResponseMessage? responseMsg)
        {
            if (responseMsg == null) return;

            HttpContext.Response.StatusCode = (int)responseMsg.StatusCode;

            if(HttpContext.Response.StatusCode != 204)
                await responseMsg.Content.CopyToAsync(HttpContext.Response.Body);
        }
    }
}
