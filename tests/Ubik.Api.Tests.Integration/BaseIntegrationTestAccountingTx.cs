using System.Net.Http.Headers;

namespace Ubik.Api.Tests.Integration
{
    internal class BaseIntegrationTestAccountingTx : BaseIntegrationTest
    {
        public BaseIntegrationTestAccountingTx(IntegrationTestProxyFactory factory) : base(factory)
        {
        }

        protected override async Task CleanupDb()
        {
            if (!Factory.IsDbCleanedAccountingSalesVatTax)
            {
                using var client = Factory.CreateClient();
                var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"/accounting/admin/api/v1/tx-app/cleanupdb");
                response.EnsureSuccessStatusCode();
                Factory.IsDbCleanedAccountingSalesVatTax = true;
            }
        }
    }
}
