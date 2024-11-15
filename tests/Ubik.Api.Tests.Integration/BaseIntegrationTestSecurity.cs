using System.Net.Http.Headers;

namespace Ubik.Api.Tests.Integration
{
    public abstract class BaseIntegrationTestSecurity : BaseIntegrationTest
    {
        internal BaseIntegrationTestSecurity(IntegrationTestProxyFactory factory) : base(factory)
        {
        }

        protected override async Task CleanupDb()
        {
            if (!Factory.IsDbCleanedSecurity)
            {
                using var client = Factory.CreateClient();
                var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"/usrmgt/admin/api/v1/application/cleanupdb");
                response.EnsureSuccessStatusCode();
                Factory.IsDbCleanedSecurity = true;
            }
        }
    }
}
