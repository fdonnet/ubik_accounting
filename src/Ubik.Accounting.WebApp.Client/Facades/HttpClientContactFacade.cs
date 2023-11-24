using System.Net.Http.Json;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Webapp.Shared.Facades;

namespace Ubik.Accounting.WebApp.Client.Facades
{
    public class HttpClientContactFacade(IHttpClientFactory httpClientFactory) : IClientContactFacade
    {
        private readonly HttpClient http = httpClientFactory.CreateClient("WebApp");

        /// <inheritdoc/>
        public async Task<IEnumerable<GetAllAccountsResult>> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            var response = await http.GetFromJsonAsync<IEnumerable<GetAllAccountsResult>>("Accountslist", cancellationToken: cancellationToken) ?? [];

            return response;
        }
    }
}
