using System.Text;
using System.Text.Json;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Webapp.Shared.Facades;

namespace Ubik.Accounting.WebApp.Client.Facades
{
    public class HttpApiAccountingFacade(IHttpClientFactory httpClientFactory) : IAccountingApiClient
    {
        private readonly HttpClient http = httpClientFactory.CreateClient("WebApp");

        public async Task<HttpResponseMessage> GetAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await http.GetAsync($"GetAccount/{id}", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("GetAllAccounts", cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> AddAccountsAsync(AddAccountCommand account, CancellationToken cancellationToken = default)
        {
            var request = JsonSerializer.Serialize(account);
            return await http.PostAsync("AddAccount", new StringContent(request, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            return await http.GetAsync("GetAllCurrencies", cancellationToken: cancellationToken);
        }
    }
}
