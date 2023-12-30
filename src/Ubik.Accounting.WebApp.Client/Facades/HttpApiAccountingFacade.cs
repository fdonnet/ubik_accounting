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
    }
}
