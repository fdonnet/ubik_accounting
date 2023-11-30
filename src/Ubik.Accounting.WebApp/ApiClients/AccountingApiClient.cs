using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.WebApp.Security;

namespace Ubik.Accounting.WebApp.ApiClients
{
    public class AccountingApiClient : IAccountingApiClient
    {
        private readonly HttpClient _client;
        private readonly UserService _user;

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

        private async Task SetSecruityHeaderAsync()
        {
            var usertoken =  await _user.GetTokenAsync();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {usertoken}");
        }
    }
}
