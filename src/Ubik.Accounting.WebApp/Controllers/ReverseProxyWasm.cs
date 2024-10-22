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
    ///
    [Authorize]
    [ApiController]
    public class ReverseProxyWasmController(IAccountingApiClient client) : ControllerBase
    {
        readonly IAccountingApiClient client = client;

        [HttpGet("/accounts")]
        public async Task AccountsList()
        {
            var response = await client.GetAllAccountsAsync();
            await ForwardResponse(response);
        }

        [HttpGet("/accounts/{id}")]
        public async Task Account(Guid id)
        {
            var response = await client.GetAccountAsync(id);
            await ForwardResponse(response);
        }

        [HttpPost("/accounts")]
        public async Task AddAccount(AddAccountCommand command)
        {
            var response = await client.AddAccountAsync(command);
            await ForwardResponse(response);
        }

        [HttpPost("/accounts/{accountId}/accountGroups/{accountGroupId}")]
        public async Task AddAccount(AddAccountInAccountGroupCommand command)
        {
            var response = await client.AddAccountInAccountGroupAsync(command);
            await ForwardResponse(response);
        }

        [HttpDelete("/accounts/{accountId}/accountGroups/{accountGroupId}")]
        public async Task DeleteAccount(Guid accountId, Guid accountGroupId)
        {
            var response = await client.DeleteAccountInAccountGroupAsync(new() { AccountGroupId = accountGroupId, AccountId = accountId });
            await ForwardResponse(response);
        }

        [HttpPut("/accounts/{id}")]
        public async Task UpdateAccount(Guid id, UpdateAccountCommand command)
        {
            var response = await client.UpdateAccountAsync(id,command);
            await ForwardResponse(response);
        }

        [HttpDelete("/accounts/{id}")]
        public async Task DeleteAccount(Guid id)
        {
            var response = await client.DeleteAccountAsync(id);
            await ForwardResponse(response);
        }

        [HttpGet("/currencies")]
        public async Task CurrenciesList()
        {
            var response = await client.GetAllCurrenciesAsync();
            await ForwardResponse(response);
        }

        [HttpGet("/classifications")]
        public async Task ClassificationsList()
        {
            var response = await client.GetAllClassificationsAsync();
            await ForwardResponse(response);
        }

        [HttpGet("/classifications/{id}/missingaccounts")]
        public async Task ClassificationMissingAccountsList(Guid id)
        {
            var response = await client.GetClassificationMissingAccountsAsync(id);
            await ForwardResponse(response);
        }

        [HttpPost("/classifications")]
        public async Task AddClassification(AddClassificationCommand command)
        {
            var response = await client.AddClassificationAsync(command);
            await ForwardResponse(response);
        }

        [HttpPut("/classifications/{id}")]
        public async Task UpdateClassification(Guid id, UpdateClassificationCommand command)
        {
            var response = await client.UpdateClassificationAsync(id, command);
            await ForwardResponse(response);
        }

        [HttpDelete("/classifications/{id}")]
        public async Task DeleteClassification(Guid id)
        {
            var response = await client.DeleteClassificationAsync(id);
            await ForwardResponse(response);
        }

        [HttpGet("/accountgroups")]
        public async Task AccountGroupsList()
        {
            var response = await client.GetAllAccountGroupsAsync();
            await ForwardResponse(response);
        }

        [HttpPost("/accountgroups")]
        public async Task AddAccountGroup(AddAccountGroupCommand command)
        {
            var response = await client.AddAccountGroupAsync(command);
            await ForwardResponse(response);
        }

        [HttpPut("/accountgroups/{id}")]
        public async Task UpdateAccountGroup(Guid id, UpdateAccountGroupCommand command)
        {
            var response = await client.UpdateAccountGroupAsync(id, command);
            await ForwardResponse(response);
        }

        [HttpDelete("/accountgroups/{id}")]
        public async Task DeleteAccountGroup(Guid id)
        {
            var response = await client.DeleteAccountGroupAsync(id);
            await ForwardResponse(response);
        }

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
