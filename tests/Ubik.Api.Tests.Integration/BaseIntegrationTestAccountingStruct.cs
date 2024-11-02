using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Api.Tests.Integration
{
    public abstract class BaseIntegrationTestAccountingStruct : BaseIntegrationTest
    {
        internal BaseIntegrationTestAccountingStruct(IntegrationTestProxyFactory factory) : base(factory)
        {
        }

        protected override async Task CleanupDb()
        {
            if (!Factory.IsDbCleanedAccounting)
            {
                using var client = Factory.CreateClient();
                var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"/accounting/admin/api/v1/application/cleanupdb");
                response.EnsureSuccessStatusCode();
                Factory.IsDbCleanedAccounting = true;
            }
        }
    }
}

