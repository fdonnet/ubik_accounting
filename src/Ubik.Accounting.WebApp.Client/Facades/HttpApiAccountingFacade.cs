using System.Text;
using System.Text.Json;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Structure.Contracts.Accounts.Commands;
using Ubik.Accounting.Structure.Contracts.Classifications.Commands;
using Ubik.Accounting.Webapp.Shared.Facades;

namespace Ubik.Accounting.WebApp.Client.Facades
{
    public class HttpApiAccountingFacade(IHttpClientFactory httpClientFactory) : IAccountingApiClient
    {
        private readonly HttpClient http = httpClientFactory.CreateClient("WebApp");

        public async Task<HttpResponseMessage> GetAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.GetAsync($"accounts/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("accounts", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountAsync(AddAccountCommand account, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(account);
            return await http.PostAsync("accounts", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountInAccountGroupAsync(AddAccountInAccountGroupCommand accountInAccountGrp, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(accountInAccountGrp);
            return await http.PostAsync($"accounts/{accountInAccountGrp.AccountId}/accountgroups/{accountInAccountGrp.AccountGroupId}"
                , new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);

        }

        public async Task<HttpResponseMessage> DeleteAccountInAccountGroupAsync(DeleteAccountInAccountGroupCommand accountInAccountGrp,
            CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"accounts/{accountInAccountGrp.AccountId}/accountgroups/{accountInAccountGrp.AccountGroupId}"
                ,  cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateAccountAsync(Guid id, UpdateAccountCommand account, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(account);
            return await http.PutAsync($"accounts/{id}", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"accounts/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("currencies", cancellationToken: cancellationToken);
        }
        public async Task<HttpResponseMessage> GetAllClassificationsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("classifications", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetClassificationMissingAccountsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.GetAsync($"classifications/{id}/missingaccounts", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddClassificationAsync(AddClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(classification);
            return await http.PostAsync("classifications", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateClassificationAsync(Guid id, UpdateClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(classification);
            return await http.PutAsync($"classifications/{id}", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteClassificationAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"classifications/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountGroupsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("accountgroups", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountGroupAsync(AddAccountGroupCommand AccountGroup, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(AccountGroup);
            return await http.PostAsync("accountgroups", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> UpdateAccountGroupAsync(Guid id, UpdateAccountGroupCommand AccountGroup, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(AccountGroup);
            return await http.PutAsync($"accountgroups/{id}", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAccountGroupAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.DeleteAsync($"accountgroups/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountsLinksAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("accounts/accountgrouplinks", cancellationToken: cancellationToken);
        }
    }
}
