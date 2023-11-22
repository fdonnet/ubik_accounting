using System.Net.Http;
using System.Net.Http.Headers;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.WebApp.Security;

namespace Ubik.Accounting.WebApp.ApiClients
{
    public class AccountingApiClient
    {
        private readonly HttpClient _client;
        //private readonly UserService _userService;

        public AccountingApiClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://api.github.com/");
            //_userService = userService;
            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_userService.GetToken().AccessToken);
        }
        public async Task<IEnumerable<GetAccountResult>> GetAllAccounts()
        {
            return await _client.GetFromJsonAsync<IEnumerable<GetAccountResult>>("Accounts") ?? [];
        }
    }
}
