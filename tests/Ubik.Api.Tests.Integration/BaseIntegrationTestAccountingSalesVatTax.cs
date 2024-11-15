using System.Net.Http.Headers;

namespace Ubik.Api.Tests.Integration
{
    public class BaseIntegrationTestAccountingSalesVatTax : BaseIntegrationTest
    {
        public BaseIntegrationTestAccountingSalesVatTax(IntegrationTestProxyFactory factory) : base(factory)
        {
        }

        protected override async Task CleanupDb()
        {
            if (!Factory.IsDbCleanedAccountingTx)
            {
                using var client = Factory.CreateClient();
                var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"/accounting/admin/api/v1/sales-vat-tax-app/cleanupdb");
                response.EnsureSuccessStatusCode();
                Factory.IsDbCleanedAccountingTx = true;
            }
        }
    }
}
