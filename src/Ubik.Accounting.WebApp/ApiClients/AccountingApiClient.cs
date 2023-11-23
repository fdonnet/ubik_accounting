using System.Net.Http;
using System.Net.Http.Headers;
using Ubik.Accounting.Contracts.Accounts.Results;
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

        public async Task<IEnumerable<GetAccountResult>> GetAllAccountsAsync()
        {
            await SetSecruityHeaderAsync();
            return await _client.GetFromJsonAsync<IEnumerable<GetAccountResult>>("Accounts") ?? [];
        }

        private async Task SetSecruityHeaderAsync()
        {
            var usertoken =  await _user.GetTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {usertoken}");
        }
    }

    public interface IAccountingApiClient
    {
        public Task<IEnumerable<GetAccountResult>> GetAllAccountsAsync();
    }
}
