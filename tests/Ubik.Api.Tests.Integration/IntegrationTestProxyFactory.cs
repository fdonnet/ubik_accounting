﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace Ubik.Api.Tests.Integration
{
    public class IntegrationTestProxyFactory
        : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly HttpClient _client = new();
        public bool IsDbCleaned { get; set; } = false;

        public IntegrationTestProxyFactory()
        {

        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            SetTestEnvVariablesForProxy();
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public new async Task DisposeAsync()
        {
            _client.Dispose();
            await Task.CompletedTask;
        }

        //private static string GetWslAbsolutePath(string windowRealtivePath)
        //{
        //    var path = Path.GetFullPath(windowRealtivePath);
        //    return "/" + path.Replace('\\', '/').Replace(":", "");
        //}

        private void SetTestEnvVariablesForProxy()
        {
            Environment.SetEnvironmentVariable("ReverseProxy__Clusters__ubik_users_admin__Destinations__destination1__Address", $"http://localhost:5000/");
            Environment.SetEnvironmentVariable("ReverseProxy__Clusters__ubik_accounting__Destinations__destination1__Address", $"http://localhost:5001/");
            Environment.SetEnvironmentVariable("ApiSecurityForAdmin__HostAndPort", $"http://localhost:5000/");
            var authTokenUrl = $"http://localhost:8082/";

            _client.BaseAddress = new Uri(authTokenUrl);
        }

    }

    [CollectionDefinition("Proxy")]
    public class KeycloackAndDb : ICollectionFixture<IntegrationTestProxyFactory>
    {

    }
}

