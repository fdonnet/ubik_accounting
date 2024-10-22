using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Commands;


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
        [HttpGet("/accounts")]
        public async Task AccountsList()
        {
            var response = await client.GetAllAccountsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("/accounts/{id}")]
        public async Task Account(Guid id)
        {
            var response = await client.GetAccountAsync(id);
            await ForwardResponse(response);
        }


        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPost("/accounts")]
        public async Task AddAccount(AddAccountCommand command)
        {
            var response = await client.AddAccountAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost("/accounts/{accountId}/accountGroups/{accountGroupId}")]
        public async Task AddAccount(AddAccountInAccountGroupCommand command)
        {
            var response = await client.AddAccountInAccountGroupAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpDelete("/accounts/{accountId}/accountGroups/{accountGroupId}")]
        public async Task DeleteAccount(Guid accountId, Guid accountGroupId)
        {
            var response = await client.DeleteAccountInAccountGroupAsync(new() { AccountGroupId = accountGroupId, AccountId = accountId });
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpPut("/accounts/{id}")]
        public async Task UpdateAccount(Guid id, UpdateAccountCommand command)
        {
            var response = await client.UpdateAccountAsync(id,command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_write")]
        [HttpDelete("/accounts/{id}")]
        public async Task DeleteAccount(Guid id)
        {
            var response = await client.DeleteAccountAsync(id);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_currency_read")]
        [HttpGet("/currencies")]
        public async Task CurrenciesList()
        {
            var response = await client.GetAllCurrenciesAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet("/classifications")]
        public async Task ClassificationsList()
        {
            var response = await client.GetAllClassificationsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_read")]
        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("/classifications/{id}/missingaccounts")]
        public async Task ClassificationMissingAccountsList(Guid id)
        {
            var response = await client.GetClassificationMissingAccountsAsync(id);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpPost("/classifications")]
        public async Task AddClassification(AddClassificationCommand command)
        {
            var response = await client.AddClassificationAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpPut("/classifications/{id}")]
        public async Task UpdateClassification(Guid id, UpdateClassificationCommand command)
        {
            var response = await client.UpdateClassificationAsync(id, command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpDelete("/classifications/{id}")]
        public async Task DeleteClassification(Guid id)
        {
            var response = await client.DeleteClassificationAsync(id);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("/accountgroups")]
        public async Task AccountGroupsList()
        {
            var response = await client.GetAllAccountGroupsAsync();
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost("/accountgroups")]
        public async Task AddAccountGroup(AddAccountGroupCommand command)
        {
            var response = await client.AddAccountGroupAsync(command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPut("/accountgroups/{id}")]
        public async Task UpdateAccountGroup(Guid id, UpdateAccountGroupCommand command)
        {
            var response = await client.UpdateAccountGroupAsync(id, command);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpDelete("/accountgroups/{id}")]
        public async Task DeleteAccountGroup(Guid id)
        {
            var response = await client.DeleteAccountGroupAsync(id);
            await ForwardResponse(response);
        }

        [Authorize(Roles = "ubik_accounting_account_read")]
        [Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet("/accounts/allaccountgrouplinks")]
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
