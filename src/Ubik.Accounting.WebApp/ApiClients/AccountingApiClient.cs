using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.WebApp.ApiClients
{
    public class AccountingApiClient(HttpClient client)
    {
        public async Task<IEnumerable<GetAccountResult>> GetAllAccounts()
        {
            return await client.GetFromJsonAsync<IEnumerable<GetAccountResult>>("Accounts") ?? [];
        }
    }
}
