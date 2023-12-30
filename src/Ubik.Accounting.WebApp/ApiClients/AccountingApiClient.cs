using System.Text;
using System.Text.Json;
using Ubik.Accounting.Contracts.Accounts.Commands;
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
            return await _client.GetAsync($"Accounts/{id}");
        }

        public async Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            return await _client.GetAsync("Accounts");
        }

        public async Task<HttpResponseMessage> AddAccountAsync(AddAccountCommand account,CancellationToken cancellationToken = default)
        {
            await SetSecruityHeaderAsync();
            var request = JsonSerializer.Serialize(account);
            return await _client.PostAsync("Accounts", new StringContent(request, Encoding.UTF8, "application/json"));
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

        private async Task SetSecruityHeaderAsync()
        {
            var usertoken =  await _user.GetTokenAsync();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {usertoken}");
        }
    }
}
