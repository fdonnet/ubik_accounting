using System.Net.Http;
using System.Net.Http.Json;
using Ubik.Accounting.Contracts.Accounts.Results;
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
    }
}
