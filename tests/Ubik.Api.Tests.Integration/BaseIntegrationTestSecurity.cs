using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Api.Tests.Integration
{
    public abstract class BaseIntegrationTestSecurity : BaseIntegrationTest
    {
        internal BaseIntegrationTestSecurity(IntegrationTestProxyFactory factory) : base(factory)
        {
        }

        internal override async Task CleanupDb()
        {
            if (!Factory.IsDbCleaned)
            {
                using var client = Factory.CreateClient();
                var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"/usrmgt/admin/api/v1/application/cleanupdb");
                response.EnsureSuccessStatusCode();
                Factory.IsDbCleaned = true;
            }
        }
    }
}
