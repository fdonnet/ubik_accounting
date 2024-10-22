using System.Text;
using System.Text.Json;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.WebApp.Security;

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
            return await _client.GetAsync($"accounts/{id}");
        }

        public async Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("accounts");
        }

        public async Task<HttpResponseMessage> AddAccountAsync(AddAccountCommand account, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(account);
            return await _client.PostAsync("accounts", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> AddAccountInAccountGroupAsync(AddAccountInAccountGroupCommand accountInAccountGrp,
            CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(accountInAccountGrp);
            return await _client.PostAsync($"accounts/{accountInAccountGrp.AccountId}/accountgroups/{accountInAccountGrp.AccountGroupId}"
                , new StringContent(request, Encoding.UTF8, "application/json"));

        }
        public async Task<HttpResponseMessage> DeleteAccountInAccountGroupAsync(DeleteAccountInAccountGroupCommand accountInAccountGrp,
            CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"accounts/{accountInAccountGrp.AccountId}/accountgroups/{accountInAccountGrp.AccountGroupId}");
        }

        public async Task<HttpResponseMessage> UpdateAccountAsync(Guid id, UpdateAccountCommand account, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(account);
            return await _client.PutAsync($"accounts/{id}", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"accounts/{id}");
        }

        public async Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("currencies");
        }

        public async Task<HttpResponseMessage> GetAllClassificationsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("classifications");
        }

        public async Task<HttpResponseMessage> GetClassificationMissingAccountsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync($"classifications/{id}/missingaccounts");
        }

        public async Task<HttpResponseMessage> AddClassificationAsync(AddClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(classification);
            return await _client.PostAsync("classifications", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateClassificationAsync(Guid id, UpdateClassificationCommand classification, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(classification);
            return await _client.PutAsync($"classifications/{id}", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteClassificationAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"classifications/{id}");
        }

        public async Task<HttpResponseMessage> GetAllAccountGroupsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("accountGroups");
        }

        public async Task<HttpResponseMessage> AddAccountGroupAsync(AddAccountGroupCommand accountGroup, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(accountGroup);
            return await _client.PostAsync("accountgroups", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateAccountGroupAsync(Guid id, UpdateAccountGroupCommand accountGroup, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(accountGroup);
            return await _client.PutAsync($"accountgroups/{id}", new StringContent(request, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteAccountGroupAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.DeleteAsync($"accountgroups/{id}");
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
            return await _client.GetAsync("accounts/allaccountgrouplinks");
        }
    }
}
