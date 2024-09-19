using System.Text;
using System.Text.Json;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.WebApp.Security;
using static System.Net.WebRequestMethods;

namespace Ubik.Accounting.WebApp.ApiClients
{
    public class AccountingApiClient : IAccountingApiClient
    {
        private readonly HttpClient _client;
        private readonly UserService _user;

        //TODO: inject Ioption for API uri
        public AccountingApiClient(HttpClient client, UserService user)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:7289/api/v1/");
            _user = user;
        }

        public async Task<HttpResponseMessage> GetAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync($"Accounts/{id}");
        }

        public async Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("Accounts");
        }

        public async Task<HttpResponseMessage> AddAccountAsync(AddAccountCommand account, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(account);
            return await _client.PostAsync("Accounts", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> AddAccountInAccountGroupAsync(AddAccountInAccountGroupCommand accountInAccountGrp,
            CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(accountInAccountGrp);
            return await _client.PostAsync($"Accounts/{accountInAccountGrp.AccountId}/AccountGroups/{accountInAccountGrp.AccountGroupId}"
                , new StringContent(request, Encoding.UTF8, "application/json"));

        }
        public async Task<HttpResponseMessage> DeleteAccountInAccountGroupAsync(DeleteAccountInAccountGroupCommand accountInAccountGrp,
            CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"Accounts/{accountInAccountGrp.AccountId}/AccountGroups/{accountInAccountGrp.AccountGroupId}");
        }

        public async Task<HttpResponseMessage> UpdateAccountAsync(Guid id, UpdateAccountCommand account, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(account);
            return await _client.PutAsync($"Accounts/{id}", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"Accounts/{id}");
        }

        public async Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("Currencies");
        }

        public async Task<HttpResponseMessage> GetAllClassificationsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("Classifications");
        }

        public async Task<HttpResponseMessage> GetClassificationMissingAccountsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync($"Classifications/{id}/MissingAccounts");
        }

        public async Task<HttpResponseMessage> AddClassificationAsync(AddClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(classification);
            return await _client.PostAsync("Classifications", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateClassificationAsync(Guid id, UpdateClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(classification);
            return await _client.PutAsync($"Classifications/{id}", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteClassificationAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"Classifications/{id}");
        }

        public async Task<HttpResponseMessage> GetAllAccountGroupsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("AccountGroups");
        }

        public async Task<HttpResponseMessage> AddAccountGroupAsync(AddAccountGroupCommand accountGroup, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(accountGroup);
            return await _client.PostAsync("AccountGroups", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateAccountGroupAsync(Guid id, UpdateAccountGroupCommand accountGroup, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(accountGroup);
            return await _client.PutAsync($"AccountGroups/{id}", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteAccountGroupAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"AccountGroups/{id}");
        }

        private async Task SetSecruityHeaderAsync()
        {
            var usertoken = await _user.GetTokenAsync();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {usertoken}");
        }

        public async Task<HttpResponseMessage> GetAllAccountsLinksAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("Accounts/AllAccountGroupLinks");
        }
    }
}
