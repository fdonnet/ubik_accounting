using System.Net.Http.Headers;

namespace Ubik.Api.Tests.Integration
{
    public abstract class BaseIntegrationTestAccountingStruct : BaseIntegrationTest
    {
        internal BaseIntegrationTestAccountingStruct(IntegrationTestProxyFactory factory) : base(factory)
        {
        }

        protected override async Task CleanupDb()
        {
            if (!Factory.IsDbCleanedAccountingStruct)
            {
                using var client = Factory.CreateClient();
                var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"/accounting/admin/api/v1/struct-app/cleanupdb");
                response.EnsureSuccessStatusCode();
                Factory.IsDbCleanedAccountingStruct = true;
            }
        }
    }
}

