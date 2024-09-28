using System.Text;
using System.Text.Json;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Webapp.Shared.Facades;

namespace Ubik.Accounting.WebApp.Client.Facades
{
    public class HttpApiAccountingFacade(IHttpClientFactory httpClientFactory) : IAccountingApiClient
    {
        private readonly HttpClient http = httpClientFactory.CreateClient("WebApp");

        public async Task<HttpResponseMessage> GetAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.GetAsync($"Accounts/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("Accounts", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountAsync(AddAccountCommand account, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(account);
            return await http.PostAsync("Accounts", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountInAccountGroupAsync(AddAccountInAccountGroupCommand accountInAccountGrp, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(accountInAccountGrp);
            return await http.PostAsync($"Accounts/{accountInAccountGrp.AccountId}/AccountGroups/{accountInAccountGrp.AccountGroupId}"
                , new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);

        }

        public async Task<HttpResponseMessage> DeleteAccountInAccountGroupAsync(DeleteAccountInAccountGroupCommand accountInAccountGrp,
            CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"Accounts/{accountInAccountGrp.AccountId}/AccountGroups/{accountInAccountGrp.AccountGroupId}"
                ,  cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateAccountAsync(Guid id, UpdateAccountCommand account, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(account);
            return await http.PutAsync($"Accounts/{id}", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"Accounts/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("Currencies", cancellationToken: cancellationToken);
        }
        public async Task<HttpResponseMessage> GetAllClassificationsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("Classifications", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetClassificationMissingAccountsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.GetAsync($"Classifications/{id}/MissingAccounts", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddClassificationAsync(AddClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(classification);
            return await http.PostAsync("Classifications", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateClassificationAsync(Guid id, UpdateClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(classification);
            return await http.PutAsync($"Classifications/{id}", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteClassificationAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"Classifications/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountGroupsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("AccountGroups", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountGroupAsync(AddAccountGroupCommand AccountGroup, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(AccountGroup);
            return await http.PostAsync("AccountGroups", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateAccountGroupAsync(Guid id, UpdateAccountGroupCommand AccountGroup, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(AccountGroup);
            return await http.PutAsync($"AccountGroups/{id}", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAccountGroupAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"AccountGroups/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountsLinksAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("Accounts/AllAccountGroupLinks", cancellationToken: cancellationToken);
        }
    }
}
